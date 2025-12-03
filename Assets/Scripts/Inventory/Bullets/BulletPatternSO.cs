using UnityEngine;

/// <summary>
/// Abstract base class for bullet patterns as ScriptableObjects.
/// Allows creating pattern assets in the editor.
/// </summary>
public abstract class BulletPatternSO : ScriptableObject, IBullet
{
    // Updated signature to include target
    public abstract void Fire(IActor source, Vector3 origin, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool, IActor target);

    /// <summary>
    /// Helper to get and initialize a projectile from the pool.
    /// </summary>
    protected void SpawnProjectile(ObjectPool<BaseProjectile> pool, BulletTypeSO config, IActor source, Vector3 position, Vector3 direction, IActor target)
    {
        BaseProjectile projectile = pool.Get();

        // Reset transform
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity;

        // Initialize logic with target
        // Default speed multiplier is 1f for player bullets
        projectile.Initialize(config, source, direction, 1f, target);
    }
}