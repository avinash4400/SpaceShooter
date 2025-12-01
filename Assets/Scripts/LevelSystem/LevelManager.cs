using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Configuration")]
    [SerializeField] private LevelSO currentLevel;
    [SerializeField] private bool autoStart = true;
    [Header("Timing")]
    [SerializeField] private float levelStartDelay = 2f;
    [SerializeField] private float levelTransitionDelay = 4f;

    private IActor playerActor;
    private int currentLevelIndex = 0;

    protected override void Awake() { base.Awake(); }

    private void OnEnable()
    {
        if (EventManager.Instance != null) EventManager.Instance.OnPlayerRegistered += InitializeWithPlayer;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null) EventManager.Instance.OnPlayerRegistered -= InitializeWithPlayer;
    }

    void Start()
    {
        if (EventManager.Instance == null) return;
        EventManager.Instance.RequestPlayer();
    }

    private void InitializeWithPlayer(IActor player)
    {
        if (playerActor != null) return;
        playerActor = player;
        if (autoStart && campaignLevels != null && campaignLevels.Count > 0)
        {
            StartCoroutine(RunCampaignRoutine());
        }
    }

    // ... RunCampaignRoutine, ProcessWave same as before ...
    // Placeholder required to avoid compilation error since fields are private in full file
    [SerializeField] private List<LevelSO> campaignLevels;
    private IEnumerator RunCampaignRoutine()
    {
        for (currentLevelIndex = 0; currentLevelIndex < campaignLevels.Count; currentLevelIndex++)
        {
            LevelSO currentLevel = campaignLevels[currentLevelIndex];
            if (currentLevel == null) continue;
            EventManager.Instance.TriggerLevelStart(currentLevel);
            yield return new WaitForSeconds(levelStartDelay);
            foreach (WaveSO wave in currentLevel.waves) yield return StartCoroutine(ProcessWave(wave));
            EventManager.Instance.TriggerLevelCompleted(currentLevel);
            yield return new WaitForSeconds(levelTransitionDelay);
        }
        EventManager.Instance.TriggerGameVictory();
    }

    private IEnumerator ProcessWave(WaveSO wave)
    {
        if (wave.startDelay > 0) yield return new WaitForSeconds(wave.startDelay);
        if (wave.runInParallel)
        {
            List<Coroutine> runningPatterns = new List<Coroutine>();
            foreach (var step in wave.spawnSteps)
                if (step.patternLogic != null) runningPatterns.Add(StartCoroutine(step.patternLogic.Execute(this, step.config)));
            foreach (var c in runningPatterns) yield return c;
        }
        else
        {
            foreach (var step in wave.spawnSteps)
                if (step.patternLogic != null) yield return StartCoroutine(step.patternLogic.Execute(this, step.config));
        }
    }

    public void SpawnEnemy(Enemy prefab, EnemyDataSO config, SpawnStrategySO strategy)
    {
        if (prefab == null || config == null || strategy == null) return;

        Vector3 spawnPos = strategy.CalculateSpawnPosition(transform);
        Enemy enemyInstance = Instantiate(prefab, spawnPos, Quaternion.identity);

        // CLEANUP: No longer injecting BulletPool here.
        // The EnemyWeapon component will fetch it from BulletManager on demand via the Strategy.
        enemyInstance.Initialize(config, playerActor);
    }
}