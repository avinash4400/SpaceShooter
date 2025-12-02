using UnityEngine;

[CreateAssetMenu(fileName = "KamikazeMovement", menuName = "Game/Enemy/Movement/Kamikaze")]
public class KamikazeMovementSO : EnemyMovementSO
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private float chargeSpeedMultiplier = 3.0f;

    [Header("Patrol (Before Lock-on)")]
    [SerializeField] private Vector3 patrolDirection = Vector3.down;
    [SerializeField] private float patrolSpeedMultiplier = 0.5f;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        // Cast state safely
        Vector3? storedPos = runtimeState as Vector3?;

        if (storedPos.HasValue)
        {
            // CHARGE
            Vector3 targetPos = storedPos.Value;
            Vector3 vectorToTarget = targetPos - currentPos;
            float distanceToTarget = vectorToTarget.magnitude;

            float chargeSpeed = speed * chargeSpeedMultiplier;
            float moveStep = chargeSpeed * Time.fixedDeltaTime;

            if (distanceToTarget <= moveStep)
            {
                return targetPos;
            }

            return currentPos + (vectorToTarget.normalized * moveStep);
        }
        else
        {
            // PATROL
            if (target != null)
            {
                float dist = Vector3.Distance(currentPos, target.GetTransform().position);
                if (dist <= detectionRadius)
                {
                    // Update state
                    runtimeState = target.GetTransform().position; // Boxing the Vector3
                    Debug.Log("Kamikaze Locked On!");
                    return currentPos;
                }
            }

            float patrolSpeed = speed * patrolSpeedMultiplier;
            return currentPos + (patrolDirection.normalized * patrolSpeed * Time.fixedDeltaTime);
        }
    }
}