using UnityEngine;

public abstract class EnemyMovementSO : ScriptableObject
{
    /// <summary>
    /// Calculates the new position for the enemy.
    /// Updated to use ref parameter for state instead of component context.
    /// </summary>
    public abstract Vector3 CalculateMovement(
        Vector3 currentPos,
        IActor target,
        float timeAlive,
        float speed,
        ref Vector3? storedPosition
    );
}