using UnityEngine;
using System;

/// <summary>
/// Core system responsible for managing the state machine and overall flow of the game.
/// It transitions between Title, StageActive, and Game Over based on global events, 
/// and reacts to the decoupled start game input signal.
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
        // Subscribe to global events (Player death and Pause/Escape)
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerDeath += OnPlayerDeath;
        }

        // Subscribe to Escape Key input
        PlayerController.OnEscapeKeyPressed += OnEscapeKeyInput;

        // NEW: Subscribe to the decoupled Start Game Input event from PlayerController
        PlayerController.OnStartGameInput += HandleStartGameInput;
    }

    void OnDisable()
    {
        // Unsubscribe
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerDeath -= OnPlayerDeath;
        }

        PlayerController.OnEscapeKeyPressed -= OnEscapeKeyInput;

        // NEW: Unsubscribe from the Start Game Input event
        PlayerController.OnStartGameInput -= HandleStartGameInput;
    }

    /// <summary>
    /// Listens for the decoupled Start Game Input event and transitions the state.
    /// </summary>
    public void HandleStartGameInput()
    {
        if (currentState == GameState.TitleScreen || currentState == GameState.GameOver || currentState == GameState.StageClear)
        {
            UpdateGameState(GameState.PreStage);
        }
    }

    /// <summary>
    /// Changes the current state of the game and broadcasts the change.
    /// </summary>
    /// <param name="newState">The state to transition to.</param>
    public void UpdateGameState(GameState newState)
    {
        if (currentState == newState) return;

        Debug.Log($"Game State Transition: {currentState} -> {newState}");
        currentState = newState;

        // Removed ControlInputMaps(newState) as that logic is now in PlayerController

        switch (newState)
        {
            case GameState.TitleScreen:
                HandleTitleScreen();
                break;
            case GameState.PreStage:
                HandlePreStage();
                break;
            case GameState.StageActive:
                HandleStageActive();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            case GameState.Pause:
                HandlePause();
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    // --- State Handlers ---

    private void HandleTitleScreen()
    {
        // Actions: Show Title UI, reset score.
    }

    private void HandlePreStage()
    {
        // Actions: Reset Player, spawn Player, initialize Score/Ammo systems
        UpdateGameState(GameState.StageActive);
    }

    private void HandleStageActive()
    {
        // Actions: Show HUD, start wave spawning, start stage timer.
    }

    private void HandleGameOver()
    {
        // Actions: Stop Spawning, halt Time, play explosion VFX/SFX, show Game Over UI.
    }

    private void HandlePause()
    {
        Time.timeScale = 0f;
        // Show Pause UI
    }

    private void HandleUnpause()
    {
        Time.timeScale = 1f;
        // Hide Pause UI
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