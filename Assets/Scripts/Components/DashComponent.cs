using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Manages the player's dash ability, including cooldown and execution.
/// Implements IGameComponent to receive its IActor owner.
/// Implements IMovementBlocker to signal movement state to PlayerMovement.
/// </summary>
public class DashComponent : MonoBehaviour, IGameComponent, IMovementBlocker
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 50f; // Increased speed for noticeable dash
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1.0f;

    // Events required by IMovementBlocker
    public event Action OnMovementBlockStart;
    public event Action OnMovementBlockEnd;

    // Events for Decoupled Communication (VFX/SFX)
    public static event Action OnDashExecuted;

    // Public properties for other scripts (like PlayerMovement) to check state
    public bool IsDashing { get; private set; } = false;
    public bool CanDash { get; private set; } = true;

    // Local IActor reference for clean abstraction
    private IActor actor;
    private Vector3 dashVelocity; // The velocity to apply during the dash

    /// <summary>
    /// Initializes the DashComponent with the owning Actor reference.
    /// Stores the reference locally.
    /// </summary>
    public void Initialize(IActor actor)
    {
        this.actor = actor;
    }

    void OnEnable()
    {
        // Subscribe to the input event
        PlayerController.OnDashAttempt += AttemptDash;
    }

    void OnDisable()
    {
        PlayerController.OnDashAttempt -= AttemptDash;
    }

    /// <summary>
    /// Attempts to execute a dash based on the current state.
    /// </summary>
    private void AttemptDash()
    {
        if (CanDash && !IsDashing)
        {
            Vector2 dashDirection = Vector2.up; // Default direction (vertical shooter)

            // *** MODIFIED LOGIC: Get current velocity from IActor and use it for direction ***
            Vector2 currentVelocity = actor.GetCurrentVelocity();

            // If the player is actively moving, use that direction. 
            // Otherwise (if standing still), default to the UP direction.
            if (currentVelocity.magnitude > 0.01f)
            {
                dashDirection = currentVelocity.normalized;
            }

            // Set the constant velocity for the dash duration
            // This is the magnitude of the dash speed multiplied by the determined direction vector.
            dashVelocity = new Vector3(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed, 0f);

            StartCoroutine(DashCoroutine());
        }
        else
        {
            // Optional: Play a "dash on cooldown" SFX
        }
    }

    private IEnumerator DashCoroutine()
    {
        if (actor == null)
        {
            Debug.LogError("[DashComponent] IActor reference is null.");
            yield break;
        }

        IsDashing = true;
        CanDash = false;

        OnMovementBlockStart?.Invoke();
        OnDashExecuted?.Invoke();

        // Enable Invulnerability
        HealthComponent health = actor.GetAttachedComponent<HealthComponent>();
        if (health != null) health.SetExternalInvulnerability(true);

        Rigidbody rb = actor.GetRigidbody();
        Vector3 dashVel = dashVelocity;
        actor.SetCurrentVelocity(dashVel);

        float timer = dashDuration;

        while (timer > 0f)
        {
            timer -= Time.fixedDeltaTime;
            rb.MovePosition(rb.position + dashVel * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        // Disable Invulnerability
        if (health != null) health.SetExternalInvulnerability(false);

        IsDashing = false;
        OnMovementBlockEnd?.Invoke();

        yield return new WaitForSeconds(dashCooldown);
        CanDash = true;
    }
}