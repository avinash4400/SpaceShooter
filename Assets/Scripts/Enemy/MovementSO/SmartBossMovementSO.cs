using UnityEngine;

/// <summary>
/// A complex AI strategy that uses a State Machine.
/// Behavior: 
/// 1. Patrols left/right.
/// 2. Dodges if bullets are near (Evasive Maneuver).
/// 3. Dives at the player occasionally.
/// </summary>
[CreateAssetMenu(fileName = "SmartBossMovement", menuName = "Game/Enemy/Movement/Boss/Smart AI")]
public class SmartBossMovementSO : EnemyMovementSO
{
    // --- Configuration ---
    [Header("Patrol")]
    [SerializeField] private float patrolWidth = 6f; // Width of movement
    [SerializeField] private float patrolSpeedMult = 1f;
    [SerializeField] private float diveCooldown = 5f;

    [Header("Evasion")]
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float evasionStrength = 8f;
    [SerializeField] private float evasionCooldown = 0.2f; // Prevents jitter
    [SerializeField] private LayerMask dangerLayers;

    [Header("Dive")]
    [SerializeField] private float diveSpeedMult = 4f;
    [SerializeField] private float diveDuration = 1.5f; // Used as a timeout
    [SerializeField] private float recoverDuration = 1.0f;

    // --- State Definition ---
    private class BossState
    {
        public enum Mode { Patrol, Dive, Recover, ReturnToAnchor }
        public Mode currentMode;
        public float stateTimer;
        public float diveTimer;
        public float evasionTimer; // Tracks dodge frequency
        public Vector3 anchorPos; // Center of patrol
        public float patrolDirection = 1f; // 1 = Right, -1 = Left
        public Vector3 diveTarget; // Locked position for the dive
    }

    private static readonly Collider[] hitBuffer = new Collider[5];

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        // 1. Initialize State
        if (runtimeState == null || !(runtimeState is BossState))
        {
            runtimeState = new BossState
            {
                currentMode = BossState.Mode.Patrol,
                anchorPos = currentPos, // Assume start pos is the anchor
                diveTimer = diveCooldown
            };
        }

        BossState state = (BossState)runtimeState;
        state.stateTimer -= Time.fixedDeltaTime;
        state.diveTimer -= Time.fixedDeltaTime;
        state.evasionTimer -= Time.fixedDeltaTime;

        Vector3 nextPos = currentPos;

        // 2. State Machine Logic
        switch (state.currentMode)
        {
            case BossState.Mode.Patrol:
                nextPos = HandlePatrol(currentPos, speed, state, target);

                // Trigger Dive?
                if (state.diveTimer <= 0 && target != null)
                {
                    state.currentMode = BossState.Mode.Dive;
                    state.stateTimer = diveDuration; // Use as timeout

                    // Lock target position immediately upon entering state
                    Vector3 tPos = target.GetTransform().position;
                    tPos.z = 0f;
                    state.diveTarget = tPos;
                }
                break;

            case BossState.Mode.Dive:
                nextPos = HandleDive(currentPos, speed, state);

                // Check arrival or timeout
                float dist = Vector3.Distance(nextPos, state.diveTarget);

                // If reached target OR timed out
                if (dist < 0.5f || state.stateTimer <= 0)
                {
                    state.currentMode = BossState.Mode.Recover;
                    state.stateTimer = recoverDuration;
                }
                break;

            case BossState.Mode.Recover:
                // Sit still or drift slightly
                if (state.stateTimer <= 0)
                {
                    state.currentMode = BossState.Mode.ReturnToAnchor;
                }
                break;

            case BossState.Mode.ReturnToAnchor:
                // Move back to start position
                nextPos = Vector3.MoveTowards(currentPos, state.anchorPos, speed * patrolSpeedMult * Time.fixedDeltaTime);
                if (Vector3.Distance(nextPos, state.anchorPos) < 0.1f)
                {
                    state.currentMode = BossState.Mode.Patrol;
                    state.diveTimer = diveCooldown; // Reset cooldown
                }
                break;
        }

        return nextPos;
    }

    private Vector3 HandlePatrol(Vector3 currentPos, float speed, BossState state, IActor target)
    {
        // A. Basic Patrol Movement (Left/Right)
        // Switch direction if too far from anchor
        if (Mathf.Abs(currentPos.x - state.anchorPos.x) > patrolWidth / 2f)
        {
            // If moving right (1) and pos > limit, switch to left (-1)
            if ((currentPos.x > state.anchorPos.x && state.patrolDirection > 0) ||
                (currentPos.x < state.anchorPos.x && state.patrolDirection < 0))
            {
                state.patrolDirection *= -1;
            }
        }

        // Calculate base movement velocity
        float moveX = state.patrolDirection * speed * patrolSpeedMult * Time.fixedDeltaTime;

        // B. Evasive Maneuver Logic (Momentum Preservation)
        // Only scan if cooldown allows
        if (state.evasionTimer <= 0)
        {
            int hits = Physics.OverlapSphereNonAlloc(currentPos, detectionRadius, hitBuffer, dangerLayers);
            if (hits > 0)
            {
                // 1. Default to current movement direction (Momentum Preservation)
                float dodgeDirection = state.patrolDirection;

                // 2. Check Screen Bounds to see if we are cornered
                if (Camera.main != null)
                {
                    Vector3 viewPos = Camera.main.WorldToViewportPoint(currentPos);

                    // If moving Right (1) but near Right Edge (> 0.9), Force Left
                    if (dodgeDirection > 0 && viewPos.x > 0.9f)
                    {
                        dodgeDirection = -1f;
                    }
                    // If moving Left (-1) but near Left Edge (< 0.1), Force Right
                    else if (dodgeDirection < 0 && viewPos.x < 0.1f)
                    {
                        dodgeDirection = 1f;
                    }
                }

                // 3. Apply Dodge (Override base movement)
                moveX = dodgeDirection * evasionStrength * Time.fixedDeltaTime;

                // Reset cooldown
                state.evasionTimer = evasionCooldown;
            }
        }

        // Apply X movement and Clamp Z to 0
        Vector3 finalPos = currentPos + new Vector3(moveX, 0, 0);
        finalPos.z = 0f;

        return finalPos;
    }

    private Vector3 HandleDive(Vector3 currentPos, float speed, BossState state)
    {
        // Move towards the LOCKED diveTarget, ignoring current player position
        Vector3 dir = (state.diveTarget - currentPos).normalized;
        return currentPos + (dir * speed * diveSpeedMult * Time.fixedDeltaTime);
    }
}