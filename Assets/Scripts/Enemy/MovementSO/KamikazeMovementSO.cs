using UnityEngine;

// Shared state class for Kamikaze logic
// Defined outside so KamikazeAttackSO can access it
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

    // Note: 'patrolDirection' field removed as it is now calculated dynamically

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        // 1. Initialize State
        if (runtimeState == null || !(runtimeState is KamikazeState))
        {
            runtimeState = new KamikazeState();
        }

        KamikazeState state = (KamikazeState)runtimeState;

        // 2. Setup Patrol Direction on First Frame
        if (!state.initialized)
        {
            if (Camera.main != null)
            {
                Vector3 vp = Camera.main.WorldToViewportPoint(currentPos);

                // Determine logic based on spawn side.
                // FIX: Check Horizontal bounds FIRST.
                // If we are off-screen Left or Right, we MUST move inward, regardless of Y height.
                if (vp.x < 0.0f)
                {
                    state.patrolDirection = Vector3.right;
                }
                else if (vp.x > 1.0f)
                {
                    state.patrolDirection = Vector3.left;
                }
                // Only if we are horizontally inside the screen do we prioritize Down for top spawns
                else if (vp.y > 0.9f)
                {
                    state.patrolDirection = Vector3.down;
                }
                else
                {
                    // Fallback for spawning inside the screen: Move towards center horizontally
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

            // FIX: Check if we are close enough to snap directly to the target
            // This prevents overshooting and flickering
            if (distanceToTarget <= moveStep)
            {
                return targetPos;
            }

            return currentPos + (vectorToTarget.normalized * moveStep);
        }
        else
        {
            // PATROL MODE
            // Check for player
            if (target != null)
            {
                float dist = Vector3.Distance(currentPos, target.GetTransform().position);

                if (dist <= detectionRadius)
                {
                    // Lock-on!
                    state.lockedTarget = target.GetTransform().position;
                    Debug.Log("Kamikaze Locked On!");
                    return currentPos;
                }
            }

            // Move in the calculated patrol direction
            float patrolSpeed = speed * patrolSpeedMultiplier;
            return currentPos + (state.patrolDirection.normalized * patrolSpeed * Time.fixedDeltaTime);
        }
    }
}