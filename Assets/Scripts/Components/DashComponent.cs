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
    [SerializeField] private float dashSpeed = 50f; 
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1.0f;

    public event Action OnMovementBlockStart;
    public event Action OnMovementBlockEnd;

    public static event Action OnDashExecuted;

  
    public bool IsDashing { get; private set; } = false;
    public bool CanDash { get; private set; } = true;

    
    private IActor actor;
    private Vector3 dashVelocity; 

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
            Vector2 dashDirection = Vector2.up; 

            Vector2 currentVelocity = actor.GetCurrentVelocity();

            if (currentVelocity.magnitude > 0.01f)
            {
                dashDirection = currentVelocity.normalized;
            }

            dashVelocity = new Vector3(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed, 0f);

            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        if (actor == null)
        {
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