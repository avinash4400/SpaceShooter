using UnityEngine;

/// <summary>
/// Strategy for enemy firing logic.
/// </summary>
public abstract class EnemyAttackSO : ScriptableObject
{
    /// <summary>
    /// Executes the attack logic.
    /// </summary>
    /// <param name="attacker">The enemy actor.</param>
    /// <param name="muzzle">The specific transform where the bullet appears.</param>
    /// <param name="target">The player target.</param>
    /// <param name="data">Configuration data (Bullet type, stats).</param>
    /// <param name="bulletPool">The pool to spawn bullets from.</param>
    /// <param name="speedMultiplier">Multiplier for bullet speed (from EnemyData).</param>
    public abstract void ExecuteAttack(
        IActor attacker,
        Transform muzzle,
        IActor target,
        EnemyDataSO data,
        ObjectPool<BaseProjectile> bulletPool,
        float speedMultiplier
    );
}