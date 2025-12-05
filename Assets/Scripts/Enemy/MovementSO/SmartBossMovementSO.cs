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
    [SerializeField] private float patrolWidth = 6f; 
    [SerializeField] private float patrolSpeedMult = 1f;
    [SerializeField] private float diveCooldown = 5f;

    [Header("Evasion")]
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float evasionStrength = 8f;
    [SerializeField] private float evasionCooldown = 0.2f; 
    [SerializeField] private LayerMask dangerLayers;

    [Header("Dive")]
    [SerializeField] private float diveSpeedMult = 4f;
    [SerializeField] private float diveDuration = 1.5f; 
    [SerializeField] private float recoverDuration = 1.0f;

    // --- State Definition ---
    private class BossState
    {
        public enum Mode { Patrol, Dive, Recover, ReturnToAnchor }
        public Mode currentMode;
        public float stateTimer;
        public float diveTimer;
        public float evasionTimer;
        public Vector3 anchorPos; 
        public float patrolDirection = 1f; 
        public Vector3 diveTarget;
    }

    private static readonly Collider[] hitBuffer = new Collider[5];

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (runtimeState == null || !(runtimeState is BossState))
        {
            runtimeState = new BossState
            {
                currentMode = BossState.Mode.Patrol,
                anchorPos = currentPos, 
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

                if (state.diveTimer <= 0 && target != null)
                {
                    state.currentMode = BossState.Mode.Dive;
                    state.stateTimer = diveDuration; 

                    Vector3 tPos = target.GetTransform().position;
                    tPos.z = 0f;
                    state.diveTarget = tPos;
                }
                break;

            case BossState.Mode.Dive:
                nextPos = HandleDive(currentPos, speed, state);

                float dist = Vector3.Distance(nextPos, state.diveTarget);

                if (dist < 0.5f || state.stateTimer <= 0)
                {
                    state.currentMode = BossState.Mode.Recover;
                    state.stateTimer = recoverDuration;
                }
                break;

            case BossState.Mode.Recover:
                if (state.stateTimer <= 0)
                {
                    state.currentMode = BossState.Mode.ReturnToAnchor;
                }
                break;

            case BossState.Mode.ReturnToAnchor:
                nextPos = Vector3.MoveTowards(currentPos, state.anchorPos, speed * patrolSpeedMult * Time.fixedDeltaTime);
                if (Vector3.Distance(nextPos, state.anchorPos) < 0.1f)
                {
                    state.currentMode = BossState.Mode.Patrol;
                    state.diveTimer = diveCooldown; 
                }
                break;
        }

        return nextPos;
    }

    private Vector3 HandlePatrol(Vector3 currentPos, float speed, BossState state, IActor target)
    {
        if (Mathf.Abs(currentPos.x - state.anchorPos.x) > patrolWidth / 2f)
        {
            if ((currentPos.x > state.anchorPos.x && state.patrolDirection > 0) ||
                (currentPos.x < state.anchorPos.x && state.patrolDirection < 0))
            {
                state.patrolDirection *= -1;
            }
        }

        float moveX = state.patrolDirection * speed * patrolSpeedMult * Time.fixedDeltaTime;

        if (state.evasionTimer <= 0)
        {
            int hits = Physics.OverlapSphereNonAlloc(currentPos, detectionRadius, hitBuffer, dangerLayers);
            if (hits > 0)
            {
                float dodgeDirection = state.patrolDirection;

                if (Camera.main != null)
                {
                    Vector3 viewPos = Camera.main.WorldToViewportPoint(currentPos);

                    if (dodgeDirection > 0 && viewPos.x > 0.9f)
                    {
                        dodgeDirection = -1f;
                    }
                    else if (dodgeDirection < 0 && viewPos.x < 0.1f)
                    {
                        dodgeDirection = 1f;
                    }
                }

                moveX = dodgeDirection * evasionStrength * Time.fixedDeltaTime;

                state.evasionTimer = evasionCooldown;
            }
        }

        Vector3 finalPos = currentPos + new Vector3(moveX, 0, 0);
        finalPos.z = 0f;

        return finalPos;
    }

    private Vector3 HandleDive(Vector3 currentPos, float speed, BossState state)
    {
        Vector3 dir = (state.diveTarget - currentPos).normalized;
        return currentPos + (dir * speed * diveSpeedMult * Time.fixedDeltaTime);
    }
}