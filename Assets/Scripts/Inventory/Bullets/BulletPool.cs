using UnityEngine;

/// <summary>
/// A specialized Object Pool for Projectiles.
/// Automatically subscribes to the OnProjectileExpired event to return objects to the pool.
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

        // The pool subscribes to the projectile's expiration event.
        // When the event fires, the pool's Return() method is called directly.
        projectile.OnProjectileExpired += Return;

        return projectile;
    }
}