using UnityEngine;

/// <summary>
/// Strategy for calculating enemy position updates.
/// </summary>
public abstract class EnemyMovementSO : ScriptableObject
{
    /// <summary>
    /// Calculates the new position for the enemy.
    /// </summary>
    /// <param name="currentPos">Current world position.</param>
    /// <param name="target">The player target (can be null).</param>
    /// <param name="timeAlive">Time since the enemy spawned.</param>
    /// <param name="speed">Movement speed stat.</param>
    /// <returns>The new position vector.</returns>
    public abstract Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed);
}