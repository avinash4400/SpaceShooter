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
    /// <param name="target">The player target.</param>
    /// <param name="data">Configuration data (Bullet type, stats).</param>
    /// <param name="bulletPool">The pool to spawn bullets from.</param>
    public abstract void ExecuteAttack(IActor attacker, IActor target, EnemyDataSO data, ObjectPool<BaseProjectile> bulletPool);
}