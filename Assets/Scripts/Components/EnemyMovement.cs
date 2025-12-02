using UnityEngine;

public class EnemyMovement : MonoBehaviour, IGameComponent
{
    private EnemyMovementSO movementStrategy;
    private EnemyRotationSO rotationStrategy;

    private IActor target;
    private float moveSpeed;
    private float timeAlive;
    private Rigidbody rb;
    private Vector2 currentVelocity;

    // Generic Memory for Strategies (The Blackboard)
    private object runtimeState;

    // Public getter if needed by attacks (cast as needed)
    public object RuntimeState => runtimeState;

    public void Initialize(IActor actor) { }

    public void Setup(EnemyMovementSO moveStrat, EnemyRotationSO rotStrat, IActor playerTarget, float speed)
    {
        movementStrategy = moveStrat;
        rotationStrategy = rotStrat;
        target = playerTarget;
        moveSpeed = speed;
        timeAlive = 0f;
        runtimeState = null; // Reset memory

        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    /// <summary>
    /// Runtime update of movement strategies (used by Boss Phases).
    /// </summary>
    public void UpdateStrategies(EnemyMovementSO newMove, EnemyRotationSO newRot)
    {
        if (newMove != null)
        {
            // If switching strategies, we generally want to reset the state memory
            // to avoid the new strategy trying to interpret old data.
            if (movementStrategy != newMove) runtimeState = null;

            movementStrategy = newMove;
        }
        if (newRot != null) rotationStrategy = newRot;
    }

    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;

        if (movementStrategy != null)
        {
            Vector3 currentPos = rb != null ? rb.position : transform.position;

            // Pass runtimeState by ref
            Vector3 nextPos = movementStrategy.CalculateMovement(currentPos, target, timeAlive, moveSpeed, ref runtimeState);

            currentVelocity = (nextPos - currentPos) / Time.fixedDeltaTime;

            if (rb != null) rb.MovePosition(nextPos);
            else transform.position = nextPos;
        }

        if (rotationStrategy != null && rb != null)
        {
            rb.rotation = rotationStrategy.CalculateRotation(rb.transform, target);
        }
    }

    public Vector2 GetVelocity() => currentVelocity;
}