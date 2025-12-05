using UnityEngine;

/// <summary>
/// A specialized Object Pool for Projectiles.
/// </summary>
public class BulletPool : ObjectPool<BaseProjectile>
{
    public BulletPool(BaseProjectile prefab, int initialSize, Transform parent)
        : base(prefab, initialSize, parent) { }

    /// <summary>
    /// Overrides the creation logic to permanently wire up the return logic.
    /// </summary>
    protected override BaseProjectile CreateNewInstance()
    {
        BaseProjectile projectile = base.CreateNewInstance();

        projectile.OnProjectileExpired += Return;

        return projectile;
    }
}