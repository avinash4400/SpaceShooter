using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Core system responsible for managing the state machine and overall flow of the game.
/// </summary>
public class GameplayManager : Singleton<GameplayManager>
{
    public static event Action<GameState> OnGameStateChanged;

    [Header("Current State")]
    [SerializeField] private GameState currentState = GameState.TitleScreen;

    [Header("Flow Settings")]
    [SerializeField] private float gameOverDuration = 3.0f;
    [Tooltip("How long to show the Game Cleared screen before returning to title.")]
    [SerializeField] private float gameVictoryDuration = 5.0f;

    void Start()
    {
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
                break;
            case GameState.GameVictory:
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
        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(gameVictoryDuration);

        UpdateGameState(GameState.TitleScreen);
    }

    private void HandleUnpause()
    {
        Time.timeScale = 1f;
        UpdateGameState(GameState.StageActive);
    }


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
        Debug.Log("Game Complete!");
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