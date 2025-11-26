using UnityEngine;
using System;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all player input and broadcasts it as events (C# Actions).
/// This component now manages enabling/disabling the "Player" and "Game" Action Maps 
/// based on signals from the GameplayManager.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Events for Player actions (used during StageActive)
    public static event Action<Vector2> OnMovementInput;
    public static event Action OnDashAttempt;

    // Events for Universal actions (used across multiple states)
    public static event Action OnEscapeKeyPressed;

    // NEW EVENT: Broadcasts that the start game input was detected.
    public static event Action OnStartGameInput;

    // Reference to the auto-generated Input Action asset class
    private GameControlSystem mGameControlSystem;

    // Public accessors for the action maps
    public InputActionMap PlayerMap => mGameControlSystem.Player.Get();
    public InputActionMap GameMap => mGameControlSystem.Game.Get();

    private void Awake()
    {
        // 1. Initialize the Input System
        mGameControlSystem = new GameControlSystem();

        // 2. Bind the Player Map Actions (Movement, Dash, EscapeKey)
        mGameControlSystem.Player.Movement.performed += OnMovementPerformed;
        mGameControlSystem.Player.Movement.canceled += OnMovementCanceled;
        mGameControlSystem.Player.Dash.performed += OnDashPerformed;
        mGameControlSystem.Player.EscapeKey.performed += OnEscapePerformed;

        // 3. Bind the Game Map Actions (StartGame)
        mGameControlSystem.Game.StartGame.performed += OnStartGamePerformed;

        // By default, only the Game map should be enabled at start (for Title Screen/StartGame)
        PlayerMap.Disable();
        GameMap.Enable();
    }

    private void OnDestroy()
    {
        if (mGameControlSystem != null)
        {
            // Unsubscribe Player Map
            mGameControlSystem.Player.Movement.performed -= OnMovementPerformed;
            mGameControlSystem.Player.Movement.canceled -= OnMovementCanceled;
            mGameControlSystem.Player.Dash.performed -= OnDashPerformed;
            mGameControlSystem.Player.EscapeKey.performed -= OnEscapePerformed;

            // Unsubscribe Game Map
            mGameControlSystem.Game.StartGame.performed -= OnStartGamePerformed;

            mGameControlSystem.Dispose();
        }
    }

    // --- Player Map Listeners (Movement & Dash) ---

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = context.ReadValue<Vector2>().normalized;
        OnMovementInput?.Invoke(inputDirection);
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        OnMovementInput?.Invoke(Vector2.zero);
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        OnDashAttempt?.Invoke();
    }

    // --- Player Map Listener (Escape) ---

    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        // This is a universal action (Pause/Quit)
        OnEscapeKeyPressed?.Invoke();
    }

    // --- Game Map Listener (StartGame) ---

    private void OnStartGamePerformed(InputAction.CallbackContext context)
    {
        // 1. Broadcast the event for GameplayManager to handle the state change
        OnStartGameInput?.Invoke();

        // 2. Explicitly switch the input maps as requested
        GameMap.Disable();
        PlayerMap.Enable();
    }
}