using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // ... (Existing Events) ...
    public static event Action<Vector2> OnMovementInput;
    public static event Action OnDashAttempt;
    public static event Action<bool> OnShootInput;
    public static event Action OnSwitchBulletInput;
    public static event Action OnSwitchPowerupInput;
    public static event Action OnActivatePowerupInput;
    public static event Action OnEscapeKeyPressed;
    public static event Action OnStartGameInput;

    private GameControlSystem mGameControlSystem;

    public InputActionMap PlayerMap => mGameControlSystem.Player.Get();
    public InputActionMap GameMap => mGameControlSystem.Game.Get();

    private void Awake()
    {
        mGameControlSystem = new GameControlSystem();

        mGameControlSystem.Player.Movement.performed += OnMovementPerformed;
        mGameControlSystem.Player.Movement.canceled += OnMovementCanceled;
        mGameControlSystem.Player.Dash.performed += OnDashPerformed;
        mGameControlSystem.Player.EscapeKey.performed += OnEscapePerformed;
        mGameControlSystem.Player.Shoot.started += OnShootStarted;
        mGameControlSystem.Player.Shoot.canceled += OnShootCanceled;

        mGameControlSystem.Player.SwitchBullet.performed += OnSwitchBulletPerformed;
        mGameControlSystem.Player.SwitchPowerup.performed += OnSwitchPowerupPerformed;
        mGameControlSystem.Player.ActivatePowerUp.performed += OnActivatePowerupPerformed;

        mGameControlSystem.Game.StartGame.performed += OnStartGamePerformed;

        PlayerMap.Disable();
        GameMap.Enable();
    }

    // ... (OnEnable/Disable/HandleGameStateChanged/OnDestroy/Listeners unchanged) ...

    private void OnEnable()
    {
        GameplayManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameplayManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameOver)
        {
            PlayerMap.Disable();
            GameMap.Enable();
        }
        else if (newState == GameState.StageActive || newState == GameState.StageClear)
        {
            PlayerMap.Enable();
            GameMap.Disable();
        }
        else if (newState == GameState.TitleScreen)
        {
            PlayerMap.Disable();
            GameMap.Enable();
        }
    }

    private void OnDestroy()
    {
        if (mGameControlSystem != null)
        {
            mGameControlSystem.Player.Movement.performed -= OnMovementPerformed;
            mGameControlSystem.Player.Movement.canceled -= OnMovementCanceled;
            mGameControlSystem.Player.Dash.performed -= OnDashPerformed;
            mGameControlSystem.Player.EscapeKey.performed -= OnEscapePerformed;
            mGameControlSystem.Player.Shoot.started -= OnShootStarted;
            mGameControlSystem.Player.Shoot.canceled -= OnShootCanceled;

            mGameControlSystem.Player.SwitchBullet.performed -= OnSwitchBulletPerformed;
            mGameControlSystem.Player.SwitchPowerup.performed -= OnSwitchPowerupPerformed;
            mGameControlSystem.Player.ActivatePowerUp.performed -= OnActivatePowerupPerformed;

            mGameControlSystem.Game.StartGame.performed -= OnStartGamePerformed;

            mGameControlSystem.Dispose();
        }
    }

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

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        OnShootInput?.Invoke(true);
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        OnShootInput?.Invoke(false);
    }

    private void OnSwitchBulletPerformed(InputAction.CallbackContext context)
    {
        OnSwitchBulletInput?.Invoke();
    }

    private void OnSwitchPowerupPerformed(InputAction.CallbackContext context)
    {
        OnSwitchPowerupInput?.Invoke();
    }

    private void OnActivatePowerupPerformed(InputAction.CallbackContext context)
    {
        OnActivatePowerupInput?.Invoke();
    }

    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        OnEscapeKeyPressed?.Invoke();
    }

    private void OnStartGamePerformed(InputAction.CallbackContext context)
    {
        OnStartGameInput?.Invoke();

        // NEW: Play UI Sound
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerUISubmit();
        }

        GameMap.Disable();
        PlayerMap.Enable();
    }
}