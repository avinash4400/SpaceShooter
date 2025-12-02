using UnityEngine;

/// <summary>
/// Strategy for enemy firing logic.
/// Holds configuration for Cooldown (Fire Rate) and Bullet Speed.
/// </summary>
public abstract class EnemyAttackSO : ScriptableObject
{
    [Header("Configuration")]
    [Tooltip("The bullet to fire for this specific attack.")]
    public BulletTypeSO bulletType;

    [Tooltip("Which muzzle on the enemy weapon to fire from.")]
    public MuzzleType muzzleType = MuzzleType.Main;

    [Header("Timing & Physics")]
    [Tooltip("Time in seconds to wait after this attack before firing again.")]
    [SerializeField] protected float attackCooldown = 1f;

    [Tooltip("Multiplier for the bullet speed. 1.0 = Default Speed from BulletConfig.")]
    [SerializeField] protected float speedMultiplier = 1f;

    /// <summary>
    /// Executes the attack logic.
    /// </summary>
    /// <returns>The time (in seconds) to wait before attacking again.</returns>
    public abstract float ExecuteAttack(
        IActor attacker,
        EnemyWeapon weapon,
        IActor target,
        EnemyDataSO data
    );

    protected ObjectPool<BaseProjectile> GetPool()
    {
        if (bulletType != null && BulletManager.Instance != null)
        {
            return BulletManager.Instance.GetPool(bulletType);
        }
        return null;
    }
}