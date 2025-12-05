using UnityEngine;


public class KamikazeState
{
    public Vector3? lockedTarget;
    public Vector3 patrolDirection;
    public bool initialized;
}

[CreateAssetMenu(fileName = "KamikazeMovement", menuName = "Game/Enemy/Movement/Kamikaze")]
public class KamikazeMovementSO : EnemyMovementSO
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private float chargeSpeedMultiplier = 3.0f;

    [Header("Patrol (Before Lock-on)")]
    [Tooltip("Speed multiplier while searching.")]
    [SerializeField] private float patrolSpeedMultiplier = 0.5f;


    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (runtimeState == null || !(runtimeState is KamikazeState))
        {
            runtimeState = new KamikazeState();
        }

        KamikazeState state = (KamikazeState)runtimeState;

        if (!state.initialized)
        {
            if (Camera.main != null)
            {
                Vector3 vp = Camera.main.WorldToViewportPoint(currentPos);

                if (vp.x < 0.0f)
                {
                    state.patrolDirection = Vector3.right;
                }
                else if (vp.x > 1.0f)
                {
                    state.patrolDirection = Vector3.left;
                }
                else if (vp.y > 0.9f)
                {
                    state.patrolDirection = Vector3.down;
                }
                else
                {
                    state.patrolDirection = (vp.x < 0.5f) ? Vector3.right : Vector3.left;
                }
            }
            else
            {
                state.patrolDirection = Vector3.down; // Fallback
            }
            state.initialized = true;
        }

        // 3. Movement Logic
        if (state.lockedTarget.HasValue)
        {
            // CHARGE MODE
            Vector3 targetPos = state.lockedTarget.Value;
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
            // PATROL MODE
            if (target != null)
            {
                float dist = Vector3.Distance(currentPos, target.GetTransform().position);

                if (dist <= detectionRadius)
                {
                    // Lock-on!
                    state.lockedTarget = target.GetTransform().position;
                    return currentPos;
                }
            }
            float patrolSpeed = speed * patrolSpeedMultiplier;
            return currentPos + (state.patrolDirection.normalized * patrolSpeed * Time.fixedDeltaTime);
        }
    }
}