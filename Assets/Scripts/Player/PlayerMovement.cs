using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Handles the core movement logic for the Player.
/// Implements IGameComponent for initialization and IMovementBlocker to signal 
/// when external components (like Dash) take over control.
/// Includes logic for smooth acceleration/friction and viewport clamping.
/// </summary>
public class PlayerMovement : MonoBehaviour, IGameComponent, IMovementBlocker
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    // Increased acceleration for snappier direction changes
    [SerializeField] private float acceleration = 80f;
    // Increased friction for immediate stop on input release
    [Range(0.1f, 1f)]
    [SerializeField] private float friction = 0.5f;
    [Header("Acceleration Curve")]
    [Tooltip("X = input magnitude, Y = acceleration multiplier")]
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);

    // Events required by IMovementBlocker
    public event Action OnMovementBlockStart;
    public event Action OnMovementBlockEnd;

    // State
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 inputDirection = Vector3.zero;
    private bool isMovementBlocked = false;

    // Constraints
    private float initialZPosition;

    // Dependency Injection
    private IActor actor;

    private Rigidbody rb;

    /// <summary>
    /// Initializes the component with the owning Actor reference.
    /// </summary>
    public void Initialize(IActor actor)
    {
        this.actor = actor;
        rb = GetComponentInChildren<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        // Set the initial Z position to lock the movement plane
        initialZPosition = actor.GetTransform().position.z;

        // Find all components that can block movement (e.g., DashComponent) and subscribe to them.
        IMovementBlocker[] movementBlockers = actor.GetTransform().GetComponents<IMovementBlocker>();

        foreach (IMovementBlocker blocker in movementBlockers)
        {
            // Ensure we don't subscribe to ourself if PlayerMovement somehow implemented a block
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

        // Unsubscribe from blockers if the component is disabled/destroyed
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

            // --- Refined Acceleration and Friction Logic (Snappier Arcade Feel) ---
            if (inputDirection.magnitude > 0.01f) // Check if input is actively being held
            {
                // Input is present: Use acceleration rate to move towards target velocity
                currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, finalAccel * Time.fixedDeltaTime);
            }
            else
            {
                // Input is absent: Use high friction rate to quickly slow down to zero
                //currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
                currentVelocity = currentVelocity * friction;
            }

            // Apply movement to the actor's Transform
            //actorTransform.position += new Vector3(currentVelocity.x, currentVelocity.y, 0f) * Time.fixedDeltaTime;
        }

        // --- Constraint Logic ---
        //ClampPlayerPosition(actorTransform);

        //// Ensure Z-position remains locked
        //if (actorTransform.position.z != initialZPosition)
        //{
        //    actorTransform.position = new Vector3(actorTransform.position.x, actorTransform.position.y, initialZPosition);
        //}

        // Share the current velocity state back to the IActor for components like Dash
        actor.SetCurrentVelocity(currentVelocity);

        // Predict next position before clamping
        Vector2 predictedPos = rb.position + currentVelocity * Time.fixedDeltaTime;

        // Clamp within camera bounds
        //predictedPos.x = Mathf.Clamp(predictedPos.x, cameraBounds.min.x + paddingX, cameraBounds.max.x - paddingX);
        //predictedPos.y = Mathf.Clamp(predictedPos.y, cameraBounds.min.y + paddingY, cameraBounds.max.y - paddingY);
        predictedPos = ClampPlayerPosition(predictedPos);
        // Move using Rigidbody
        rb.MovePosition(predictedPos);
    }

    /// <summary>
    /// Clamps the player's position within the camera's viewport boundaries.
    /// </summary>
    private void ClampPlayerPosition(Transform targetTransform)
    {
        if (Camera.main == null) return;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(targetTransform.position);

        // Clamp view coordinates between 5% (0.05) and 95% (0.95)
        viewPos.x = Mathf.Clamp(viewPos.x, 0.05f, 0.95f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0.05f, 0.95f);

        Vector3 newWorldPosition = Camera.main.ViewportToWorldPoint(viewPos);

        // Apply the clamped position, retaining the fixed initial Z position
        targetTransform.position = new Vector3(newWorldPosition.x, newWorldPosition.y, initialZPosition);
    }

    /// <summary>
    /// Clamps the player's position within the camera's viewport boundaries.
    /// </summary>
    private Vector3 ClampPlayerPosition(Vector2 targetPosition)
    {
        if (Camera.main == null) return targetPosition;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(targetPosition);

        // Clamp view coordinates between 5% (0.05) and 95% (0.95)
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