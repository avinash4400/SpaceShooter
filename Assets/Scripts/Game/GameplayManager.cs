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
            EventManager.Instance.OnLevelCompleted += OnLevelCompleted; // Listen for victory
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
                // HandleTitleScreen();
                break;
            case GameState.PreStage:
                // HandlePreStage();
                // Typically waits for initialization then goes to Active
                UpdateGameState(GameState.StageActive);
                break;
            case GameState.StageActive:
                // HandleStageActive();
                break;
            case GameState.GameOver:
                // HandleGameOver();
                break;
            case GameState.StageClear:
                // HandleStageClear();
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
        // When level is finished, transition to StageClear
        if (currentState == GameState.StageActive)
        {
            Debug.Log("Level Complete! Transitioning to StageClear.");
            UpdateGameState(GameState.StageClear);
        }
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