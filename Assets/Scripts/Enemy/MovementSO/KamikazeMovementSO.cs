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

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref Vector3? storedPosition)
    {
        // 1. Check if we already have a locked target (via ref)
        if (storedPosition.HasValue)
        {
            // CHARGE MODE
            Vector3 targetPos = storedPosition.Value;
            Vector3 vectorToTarget = targetPos - currentPos;
            float distanceToTarget = vectorToTarget.magnitude;

            float chargeSpeed = speed * chargeSpeedMultiplier;
            float moveStep = chargeSpeed * Time.fixedDeltaTime;

            // FIX: Check if we are close enough to snap directly to the target
            // This prevents overshooting and flickering
            if (distanceToTarget <= moveStep)
            {
                return targetPos;
            }

            // Otherwise move normally
            return currentPos + (vectorToTarget.normalized * moveStep);
        }
        else
        {
            // PATROL MODE
            // Check for player
            if (target != null)
            {
                float dist = Vector3.Distance(currentPos, target.GetTransform().position);
                // Added explicit check !storedPosition.HasValue to ensure we only lock on once
                if (dist <= detectionRadius && !storedPosition.HasValue)
                {
                    // Lock-on! Update the ref value
                    storedPosition = target.GetTransform().position;
                    Debug.Log("Kamikaze Locked On!");
                    return currentPos;
                }
            }

            // Move normally
            float patrolSpeed = speed * patrolSpeedMultiplier;
            return currentPos + (patrolDirection.normalized * patrolSpeed * Time.fixedDeltaTime);
        }
    }
}