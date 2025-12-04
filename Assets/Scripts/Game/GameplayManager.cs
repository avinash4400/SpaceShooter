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

    [Header("Flow Settings")]
    [SerializeField] private float gameOverDuration = 3.0f;
    [Tooltip("How long to show the Game Cleared screen before returning to title.")]
    [SerializeField] private float gameVictoryDuration = 5.0f;

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
    }

    public void UpdateGameState(GameState newState)
    {
        if (currentState == newState) return;

        Debug.Log($"Game State Transition: {currentState} -> {newState}");
        currentState = newState;

        switch (newState)
        {
            case GameState.TitleScreen:
                Time.timeScale = 1f;
                break;
            case GameState.PreStage:
                UpdateGameState(GameState.StageActive);
                break;
            case GameState.StageActive:
                Time.timeScale = 1f;
                break;
            case GameState.GameOver:
                StartCoroutine(HandleGameOverSequence());
                break;
            case GameState.StageClear:
                // Just wait for LevelManager to load next level
                break;
            case GameState.GameVictory:
                // All levels done. Start sequence to return to title.
                StartCoroutine(HandleGameVictorySequence());
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private IEnumerator HandleGameOverSequence()
    {
        yield return new WaitForSecondsRealtime(gameOverDuration);
        UpdateGameState(GameState.TitleScreen);
    }

    private IEnumerator HandleGameVictorySequence()
    {
        // Keep game running or pause it? Usually nice to see fireworks/particles move.
        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(gameVictoryDuration);

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
        UpdateGameState(GameState.StageActive);
    }

    private void OnGameVictory()
    {
        Debug.Log("Campaign Complete!");
        UpdateGameState(GameState.GameVictory);
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