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

    // Memory for Strategies (Encapsulated)
    private Vector3? storedPosition;

    public Vector3? StoredPosition => storedPosition;

    public void Initialize(IActor actor) { }

    public void Setup(EnemyMovementSO moveStrat, EnemyRotationSO rotStrat, IActor playerTarget, float speed)
    {
        movementStrategy = moveStrat;
        rotationStrategy = rotStrat;
        target = playerTarget;
        moveSpeed = speed;
        timeAlive = 0f;
        storedPosition = null;

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
        if (newMove != null) movementStrategy = newMove;
        if (newRot != null) rotationStrategy = newRot;

        // Optionally reset memory when switching strategies?
        // storedPosition = null; 
    }

    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;

        if (movementStrategy != null)
        {
            Vector3 currentPos = rb != null ? rb.position : transform.position;

            Vector3 nextPos = movementStrategy.CalculateMovement(currentPos, target, timeAlive, moveSpeed, ref storedPosition);

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