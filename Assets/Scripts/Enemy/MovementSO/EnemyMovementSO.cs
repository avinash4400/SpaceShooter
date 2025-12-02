using UnityEngine;

/// <summary>
/// Strategy for calculating enemy position updates.
/// </summary>
public abstract class EnemyMovementSO : ScriptableObject
{
    /// <summary>
    /// Calculates the new position for the enemy.
    /// Uses 'runtimeState' as a generic blackboard for the strategy to store memory (Timer, State Machine, etc).
    /// </summary>
    public abstract Vector3 CalculateMovement(
        Vector3 currentPos,
        IActor target,
        float timeAlive,
        float speed,
        ref object runtimeState
    );
}