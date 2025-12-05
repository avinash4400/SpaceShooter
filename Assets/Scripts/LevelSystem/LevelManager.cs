using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The central director for the game level.
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    [Header("Campaign Configuration")]
    [Tooltip("The list of levels to play in order.")]
    [SerializeField] private List<LevelSO> campaignLevels;

    [SerializeField] private bool autoStart = true;

    [Header("Timing")]
    [Tooltip("Time to wait after level start before spawning the first wave.")]
    [SerializeField] private float levelStartDelay = 2f;

    [Tooltip("Time to wait after a level finishes before loading the next one.")]
    [SerializeField] private float levelTransitionDelay = 4f;

    private IActor playerActor;
    private int currentLevelIndex = 0;

    private int activeEnemyCount = 0;
    public int ActiveEnemyCount => activeEnemyCount;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerRegistered += InitializeWithPlayer;
            EventManager.Instance.OnEnemySpawned += HandleEnemySpawned;
            EventManager.Instance.OnEnemyDespawned += HandleEnemyDespawned;
        }

        GameplayManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerRegistered -= InitializeWithPlayer;
            EventManager.Instance.OnEnemySpawned -= HandleEnemySpawned;
            EventManager.Instance.OnEnemyDespawned -= HandleEnemyDespawned;
        }

        GameplayManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameOver)
        {
            StopAllCoroutines();
        }
        else if (newState == GameState.TitleScreen || newState == GameState.PreStage)
        {
            CleanupScene();
        }
    }

    /// <summary>
    /// Destroys all active enemies, bullets, and pickups to reset the stage.
    /// </summary>
    private void CleanupScene()
    {
        StopAllCoroutines(); 

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            if (e != null) Destroy(e.gameObject);
        }

        BaseProjectile[] bullets = FindObjectsByType<BaseProjectile>(FindObjectsSortMode.None);
        foreach (var b in bullets)
        {
            if (b != null) Destroy(b.gameObject);
        }

        BasePickup[] pickups = FindObjectsByType<BasePickup>(FindObjectsSortMode.None);
        foreach (var p in pickups)
        {
            if (p != null) Destroy(p.gameObject);
        }

        currentLevelIndex = 0;
        activeEnemyCount = 0;

        Debug.Log("[LevelManager] Scene Cleaned.");
    }

    private void HandleEnemySpawned(Enemy enemy)
    {
        activeEnemyCount++;
    }

    private void HandleEnemyDespawned(Enemy enemy)
    {
        activeEnemyCount--;
        if (activeEnemyCount < 0) activeEnemyCount = 0;
    }

    void Start()
    {
        if (EventManager.Instance == null)
        {
            Debug.LogError("[LevelManager] EventManager is missing! Cannot start handshake.");
            return;
        }
        EventManager.Instance.RequestPlayer();
    }

    private void InitializeWithPlayer(IActor player)
    {
        if (playerActor == player) return;

        playerActor = player;

        if (autoStart && campaignLevels != null && campaignLevels.Count > 0)
        {
            StartCoroutine(RunCampaignRoutine());
        }
    }

    private IEnumerator RunCampaignRoutine()
    {
        for (currentLevelIndex = 0; currentLevelIndex < campaignLevels.Count; currentLevelIndex++)
        {
            LevelSO currentLevel = campaignLevels[currentLevelIndex];

            if (currentLevel == null) continue;

            Debug.Log($"[LevelManager] Starting Level {currentLevelIndex + 1}: {currentLevel.levelName}");

            EventManager.Instance.TriggerLevelStart(currentLevel);

            yield return new WaitForSeconds(levelStartDelay);

            foreach (WaveSO wave in currentLevel.waves)
            {
                yield return StartCoroutine(ProcessWave(wave));
            }

            Debug.Log($"[LevelManager] Level {currentLevel.levelName} Complete!");

            EventManager.Instance.TriggerLevelCompleted(currentLevel);

            yield return new WaitForSeconds(levelTransitionDelay);
        }

        Debug.Log("[LevelManager] Game Victory!");
        EventManager.Instance.TriggerGameVictory();
    }

    private IEnumerator ProcessWave(WaveSO wave)
    {
        Debug.LogWarningFormat($"[LevelManager] Starting Wave: {wave.name}");
        if (wave.startDelay > 0) yield return new WaitForSeconds(wave.startDelay);

        if (wave.runInParallel)
        {
            List<Coroutine> runningPatterns = new List<Coroutine>();
            foreach (var step in wave.spawnSteps)
            {
                if (step.patternLogic != null)
                {
                    runningPatterns.Add(StartCoroutine(step.patternLogic.Execute(this, step.config)));
                }
            }

            foreach (var c in runningPatterns) yield return c;
        }
        else
        {
            foreach (var step in wave.spawnSteps)
            {
                if (step.patternLogic != null)
                {
                    yield return StartCoroutine(step.patternLogic.Execute(this, step.config));
                }
            }
        }
    }

    public void SpawnEnemy(Enemy prefab, EnemyDataSO config, SpawnStrategySO strategy)
    {
        if (prefab == null || config == null || strategy == null) return;

        Vector3 spawnPos = strategy.CalculateSpawnPosition(transform);
        Enemy enemyInstance = Instantiate(prefab, spawnPos, Quaternion.identity);

        BulletPool pool = null;
        if (config.attackPattern != null && config.attackPattern.bulletType != null && BulletManager.Instance != null)
        {
            pool = BulletManager.Instance.GetPool(config.attackPattern.bulletType);
        }

        enemyInstance.Initialize(config, playerActor, pool);
    }
}