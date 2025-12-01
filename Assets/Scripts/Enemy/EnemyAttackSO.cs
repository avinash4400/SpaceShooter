using UnityEngine;

/// <summary>
/// Strategy for enemy firing logic.
/// Updated to handle Muzzle and Bullet selection internally.
/// </summary>
public abstract class EnemyAttackSO : ScriptableObject
{
    [Header("Configuration")]
    [Tooltip("The bullet to fire for this specific attack.")]
    public BulletTypeSO bulletType;

    [Tooltip("Which muzzle on the enemy weapon to fire from.")]
    public MuzzleType muzzleType = MuzzleType.Main;

    /// <summary>
    /// Executes the attack logic.
    /// </summary>
    /// <param name="attacker">The enemy actor.</param>
    /// <param name="weapon">The weapon component to query for muzzles.</param>
    /// <param name="target">The player target.</param>
    /// <param name="data">Enemy config data.</param>
    /// <param name="speedMultiplier">Speed modifier.</param>
    /// <returns>Cooldown time.</returns>
    public abstract float ExecuteAttack(
        IActor attacker,
        EnemyWeapon weapon,
        IActor target,
        EnemyDataSO data,
        float speedMultiplier
    );

    /// <summary>
    /// Helper to get the correct pool from the global manager.
    /// </summary>
    protected ObjectPool<BaseProjectile> GetPool()
    {
        if (bulletType != null && BulletManager.Instance != null)
        {
            return BulletManager.Instance.GetPool(bulletType);
        }
        return null;
    }
}