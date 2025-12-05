using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Handles the core movement logic for the Player.
/// </summary>
public class PlayerMovement : MonoBehaviour, IGameComponent, IMovementBlocker
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float acceleration = 80f;
    [Range(0.1f, 1f)]
    [SerializeField] private float friction = 0.5f;
    [Header("Acceleration Curve")]
    [Tooltip("X = input magnitude, Y = acceleration multiplier")]
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);

    // Events required by IMovementBlocker
    public event Action OnMovementBlockStart;
    public event Action OnMovementBlockEnd;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 inputDirection = Vector3.zero;
    private bool isMovementBlocked = false;

    private float initialZPosition;

    private IActor actor;

    private Rigidbody rb;

    /// <summary>
    /// Initializes the component with the owning Actor reference.
    /// </summary>
    public void Initialize(IActor actor)
    {
        this.actor = actor;
        rb = actor.GetRigidbody();
        rb.useGravity = false;
        rb.freezeRotation = true;
        initialZPosition = actor.GetTransform().position.z;

        IMovementBlocker[] movementBlockers = actor.GetTransform().GetComponents<IMovementBlocker>();

        foreach (IMovementBlocker blocker in movementBlockers)
        {
            if (blocker != (IMovementBlocker)this)
            {
                blocker.OnMovementBlockStart += SetMovementBlocked;
                blocker.OnMovementBlockEnd += SetMovementUnblocked;
            }
        }
    }

    void OnEnable()
    {
        PlayerController.OnMovementInput += OnMovementInputReceived;
    }

    void OnDisable()
    {
        PlayerController.OnMovementInput -= OnMovementInputReceived;

        if (actor != null)
        {
            IMovementBlocker[] movementBlockers = actor.GetTransform().GetComponents<IMovementBlocker>();
            foreach (IMovementBlocker blocker in movementBlockers)
            {
                if (blocker != (IMovementBlocker)this)
                {
                    blocker.OnMovementBlockStart -= SetMovementBlocked;
                    blocker.OnMovementBlockEnd -= SetMovementUnblocked;
                }
            }
        }
    }

    /// <summary>
    /// Event listener for movement input from PlayerController.
    /// </summary>
    /// <param name="direction">Normalized input vector (from WASD/stick).</param>
    private void OnMovementInputReceived(Vector2 direction)
    {
        inputDirection = direction;
    }

    void FixedUpdate()
    {
        if (actor == null) return;
        Transform actorTransform = actor.GetTransform();

        if (!isMovementBlocked)
        {
            Vector2 targetVelocity = inputDirection * speed;
            float accelMult = accelerationCurve.Evaluate(inputDirection.magnitude);

            float finalAccel = acceleration * accelMult;

            if (inputDirection.magnitude > 0.01f) 
            {
                currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, finalAccel * Time.fixedDeltaTime);
            }
            else
            {
                currentVelocity = currentVelocity * friction;
                currentVelocity *= Mathf.Pow(friction, Time.fixedDeltaTime);
            }

        }

        actor.SetCurrentVelocity(currentVelocity);

        if(currentVelocity.magnitude > 0.01f)
        {
            Vector2 predictedPos = rb.position + currentVelocity * Time.fixedDeltaTime;
            predictedPos = ClampPlayerPosition(predictedPos);
            rb.MovePosition(predictedPos);
        }
        
    }

    /// <summary>
    /// Clamps the player's position within the camera's viewport boundaries.
    /// </summary>
    private void ClampPlayerPosition(Transform targetTransform)
    {
        if (Camera.main == null) return;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(targetTransform.position);

        viewPos.x = Mathf.Clamp(viewPos.x, 0.05f, 0.95f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0.05f, 0.95f);

        Vector3 newWorldPosition = Camera.main.ViewportToWorldPoint(viewPos);

        targetTransform.position = new Vector3(newWorldPosition.x, newWorldPosition.y, initialZPosition);
    }

    /// <summary>
    /// Clamps the player's position within the camera's viewport boundaries.
    /// </summary>
    private Vector3 ClampPlayerPosition(Vector2 targetPosition)
    {
        if (Camera.main == null) return targetPosition;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(targetPosition);

        viewPos.x = Mathf.Clamp(viewPos.x, 0.05f, 0.95f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0.05f, 0.95f);

        Vector3 newWorldPosition = Camera.main.ViewportToWorldPoint(viewPos);
        return newWorldPosition;
    }

    /// <summary>
    /// Called when another component (e.g., Dash) takes over movement control.
    /// </summary>
    private void SetMovementBlocked()
    {
        isMovementBlocked = true;
    }

    /// <summary>
    /// Called when the component blocking movement is finished.
    /// </summary>
    private void SetMovementUnblocked()
    {
        isMovementBlocked = false;
    }
}