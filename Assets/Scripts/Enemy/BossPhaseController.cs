using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages Boss behavior states based on health thresholds.
/// Attaches to the Boss Prefab alongside the Enemy component.
/// </summary>
public class BossPhaseController : MonoBehaviour
{
    [System.Serializable]
    public struct BossPhase
    {
        [Range(0f, 1f)]
        [Tooltip("Trigger this phase when Health % is below this value.")]
        public float healthThreshold;

        public EnemyMovementSO movementPattern;
        public EnemyRotationSO rotationPattern;
        public EnemyAttackSO attackPattern;
    }

    [Header("Phase Configuration")]
    [Tooltip("List of phases. Should be ordered from highest threshold (e.g. 0.75) to lowest (0.25).")]
    [SerializeField] private List<BossPhase> phases;

    // References
    private Enemy enemyController;
    private HealthComponent health;
    private int currentPhaseIndex = -1;

    void Start()
    {
        enemyController = GetComponent<Enemy>();
        health = GetComponent<HealthComponent>();

        if (health != null)
        {
            health.OnHealthChanged += CheckPhases;

            // Notify UI
            if (EventManager.Instance != null)
            {
                EventManager.Instance.TriggerBossSpawned(health);
            }
        }
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthChanged -= CheckPhases;
        }
    }

    private void CheckPhases(GameObject source, int currentHealth)
    {
        if (phases == null || phases.Count == 0) return;

        float healthPercent = (float)currentHealth / health.MaxHealth;

        // Iterate to find the active phase
        // We look for the first phase in the list that meets the threshold condition 
        // that we haven't already entered.

        for (int i = 0; i < phases.Count; i++)
        {
            // If we haven't reached this phase yet AND health is low enough
            if (i > currentPhaseIndex && healthPercent <= phases[i].healthThreshold)
            {
                EnterPhase(i);
                currentPhaseIndex = i; // Mark this as the current active phase
            }
        }
    }

    private void EnterPhase(int index)
    {
        BossPhase phase = phases[index];
        Debug.Log($"[Boss] Entering Phase {index + 1} at {phase.healthThreshold * 100}% HP");

        if (enemyController != null)
        {
            enemyController.OverrideMovement(phase.movementPattern, phase.rotationPattern);
            enemyController.OverrideAttack(phase.attackPattern);
        }
    }
}