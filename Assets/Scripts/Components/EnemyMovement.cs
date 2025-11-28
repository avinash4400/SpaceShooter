using UnityEngine;

/// <summary>
/// Handles the physical movement and rotation of the enemy based on Strategies.
/// Controlled by the main Enemy script.
/// </summary>
public class EnemyMovement : MonoBehaviour, IGameComponent
{
    // Strategies
    private EnemyMovementSO movementStrategy;
    private EnemyRotationSO rotationStrategy;

    // State
    private IActor target;
    private float moveSpeed;
    private float timeAlive;
    private Rigidbody rb;

    // Internal Velocity tracking for Rotation Strategy
    private Vector2 currentVelocity;

    public void Initialize(IActor actor)
    {
        // RB is set up in Setup() called by Enemy.cs
    }

    public void Setup(EnemyMovementSO moveStrat, EnemyRotationSO rotStrat, IActor playerTarget, float speed)
    {
        movementStrategy = moveStrat;
        rotationStrategy = rotStrat;
        target = playerTarget;
        moveSpeed = speed;
        timeAlive = 0f;

        // Use GetComponentInChildren to find the Rigidbody if it's on a child
        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;

        // 1. Calculate and Apply Movement
        if (movementStrategy != null)
        {
            // Use Rigidbody position as the source of truth
            Vector3 currentPos = rb != null ? rb.position : transform.position;

            Vector3 nextPos = movementStrategy.CalculateMovement(currentPos, target, timeAlive, moveSpeed);

            // Calculate velocity
            currentVelocity = (nextPos - currentPos) / Time.fixedDeltaTime;

            if (rb != null)
            {
                rb.MovePosition(nextPos);
            }
            else
            {
                transform.position = nextPos;
            }
        }

        // 2. Calculate and Apply Rotation
        if (rotationStrategy != null && rb != null)
        {
            // Apply rotation to the Rigidbody transform (the visual/physical object)
            rb.rotation = rotationStrategy.CalculateRotation(rb.transform, target);
        }
    }

    public Vector2 GetVelocity() => currentVelocity;
}