using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The central director for the game level.
/// Uses the Handshake Pattern to acquire the Player reference safely.
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    [Header("Configuration")]
    [SerializeField] private LevelSO currentLevel;
    [SerializeField] private bool autoStart = true;

    // State
    private IActor playerActor;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerRegistered += InitializeWithPlayer;
        }
    }

    void Start()
    {
        if (EventManager.Instance == null)
        {
            Debug.LogError("[LevelManager] EventManager is missing! Cannot start handshake.");
            return;
        }

        // Start Handshake
        //Removed Handshake request
        //EventManager.Instance.RequestPlayer();
    }

    /// <summary>
    /// Callback when the Player responds to the handshake.
    /// </summary>
    private void InitializeWithPlayer(IActor player)
    {
        EventManager.Instance.OnPlayerRegistered -= InitializeWithPlayer;
        // Prevent re-initialization
        if (playerActor != null) return;

        playerActor = player;
        Debug.Log($"[LevelManager] Player acquired: {player.GetTransform().name}");

        if (autoStart && currentLevel != null)
        {
            StartCoroutine(RunLevelRoutine());
        }
    }

    // --- Execution Logic ---

    private IEnumerator RunLevelRoutine()
    {
        Debug.Log($"[LevelManager] Starting Level: {currentLevel.levelName}");

        // Notify systems (UI, Audio) that level has begun
        EventManager.Instance.TriggerLevelStart(currentLevel);

        yield return new WaitForSeconds(2f); // Intro delay

        foreach (WaveSO wave in currentLevel.waves)
        {
            yield return StartCoroutine(ProcessWave(wave));
        }

        Debug.Log("[LevelManager] Level Complete!");

        // Notify systems that level is finished (triggers Stage Clear)
        EventManager.Instance.TriggerLevelCompleted(currentLevel);
    }

    private IEnumerator ProcessWave(WaveSO wave)
    {
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

            foreach (var c in runningPatterns)
            {
                yield return c;
            }
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

    // --- Service Methods for Patterns ---

    public void SpawnEnemy(Enemy prefab, EnemyDataSO config, SpawnStrategySO strategy)
    {
        if (prefab == null || config == null || strategy == null) return;

        Vector3 spawnPos = strategy.CalculateSpawnPosition(transform);
        Enemy enemyInstance = Instantiate(prefab, spawnPos, Quaternion.identity);
        enemyInstance.Initialize(config, playerActor);
    }
}