using UnityEngine;
using System;

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
            EventManager.Instance.OnLevelStarted += OnLevelStarted; // New listener
            EventManager.Instance.OnGameVictory += OnGameVictory;   // New listener
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
        if (currentState == GameState.TitleScreen || currentState == GameState.GameOver || currentState == GameState.StageClear)
        {
            UpdateGameState(GameState.PreStage);
        }
    }

    public void UpdateGameState(GameState newState)
    {
        if (currentState == newState) return;

        Debug.Log($"Game State Transition: {currentState} -> {newState}");
        currentState = newState;

        switch (newState)
        {
            case GameState.TitleScreen:
                break;
            case GameState.PreStage:
                // PreStage usually handles setup, then immediately goes to Active 
                // OR waits for LevelManager to fire OnLevelStarted.
                // For now, we allow LevelManager's event to trigger StageActive.
                break;
            case GameState.StageActive:
                // Enable Input, Hide Cursor, etc.
                break;
            case GameState.GameOver:
                // Show Game Over UI
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
        // Separate state for finishing the entire campaign
        // For simplicity, we can use StageClear or a dedicated Victory state
        // Assuming GameState enum has GameVictory, otherwise fallback to StageClear
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