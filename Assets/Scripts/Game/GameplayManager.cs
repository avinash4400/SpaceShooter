using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Core system responsible for managing the state machine and overall flow of the game.
/// It transitions between Title, StageActive, and Game Over based on global events.
/// Inherits from Singleton to ensure a single instance persists across scenes.
/// </summary>
public class GameplayManager : Singleton<GameplayManager>
{
    // Public event for other systems to react to state changes (e.g., UI, Spawner)
    public static event Action<GameState> OnGameStateChanged;

    [Header("Current State")]
    [SerializeField] private GameState currentState = GameState.TitleScreen;

    [Header("Game Over Settings")]
    [SerializeField] private float gameOverDuration = 3.0f;

    void Start()
    {
        // Start the game loop on the Title Screen
        UpdateGameState(GameState.TitleScreen);
    }

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerDeath += OnPlayerDeath;
            EventManager.Instance.OnLevelCompleted += OnLevelCompleted;
            EventManager.Instance.OnLevelStarted += OnLevelStarted;
            EventManager.Instance.OnGameVictory += OnGameVictory;
        }

        PlayerController.OnEscapeKeyPressed += OnEscapeKeyInput;
        PlayerController.OnStartGameInput += HandleStartGameInput;
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerDeath -= OnPlayerDeath;
            EventManager.Instance.OnLevelCompleted -= OnLevelCompleted;
            EventManager.Instance.OnLevelStarted -= OnLevelStarted;
            EventManager.Instance.OnGameVictory -= OnGameVictory;
        }

        PlayerController.OnEscapeKeyPressed -= OnEscapeKeyInput;
        PlayerController.OnStartGameInput -= HandleStartGameInput;
    }

    public void HandleStartGameInput()
    {
        if (currentState == GameState.TitleScreen || currentState == GameState.StageClear)
        {
            UpdateGameState(GameState.PreStage);
        }
        // Note: We don't allow restart from GameOver here immediately, 
        // as the coroutine handles the transition back to Title.
    }

    public void UpdateGameState(GameState newState)
    {
        if (currentState == newState) return;

        Debug.Log($"Game State Transition: {currentState} -> {newState}");
        currentState = newState;

        switch (newState)
        {
            case GameState.TitleScreen:
                // Reset Time Scale in case we came from Pause
                Time.timeScale = 1f;
                break;
            case GameState.PreStage:
                // HandlePreStage();
                // Typically waits for initialization then goes to Active
                UpdateGameState(GameState.StageActive);
                break;
            case GameState.StageActive:
                Time.timeScale = 1f;
                break;
            case GameState.GameOver:
                // Start the sequence to return to main menu
                StartCoroutine(HandleGameOverSequence());
                break;
            case GameState.StageClear:
                // Show Victory UI, Stop Timer
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private IEnumerator HandleGameOverSequence()
    {
        // Wait for the duration (Realtime, in case we want to slowmo the game)
        yield return new WaitForSecondsRealtime(gameOverDuration);

        // Return to Title Screen
        UpdateGameState(GameState.TitleScreen);
    }

    private void HandleUnpause()
    {
        Time.timeScale = 1f;
        UpdateGameState(GameState.StageActive);
    }

    // --- Event Listeners ---

    private void OnPlayerDeath()
    {
        if (currentState == GameState.StageActive)
        {
            UpdateGameState(GameState.GameOver);
        }
    }

    private void OnLevelCompleted(LevelSO level)
    {
        if (currentState == GameState.StageActive)
        {
            Debug.Log("Level Complete! Transitioning to StageClear.");
            UpdateGameState(GameState.StageClear);
        }
    }

    private void OnLevelStarted(LevelSO level)
    {
        // When a new level starts, ensure we are in the Active state (hiding stage clear UI)
        UpdateGameState(GameState.StageActive);
    }

    private void OnGameVictory()
    {
        Debug.Log("Campaign Complete!");
        UpdateGameState(GameState.StageClear);
    }

    private void OnEscapeKeyInput()
    {
        if (currentState == GameState.StageActive)
        {
            UpdateGameState(GameState.Pause);
        }
        else if (currentState == GameState.Pause)
        {
            HandleUnpause();
        }
    }
}