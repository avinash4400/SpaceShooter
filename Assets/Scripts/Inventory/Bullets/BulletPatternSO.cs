using UnityEngine;

/// <summary>
/// Abstract base class for bullet patterns as ScriptableObjects.
/// Allows creating pattern assets in the editor.
/// </summary>
public abstract class BulletPatternSO : ScriptableObject, IBullet
{
    public abstract void Fire(IActor source, Vector3 origin, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool);

    /// <summary>
    /// Helper to get and initialize a projectile from the pool.
    /// </summary>
    protected void SpawnProjectile(ObjectPool<BaseProjectile> pool, BulletTypeSO config, IActor source, Vector3 position, Vector3 direction)
    {
        BaseProjectile projectile = pool.Get();

        // Reset transform
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity; // Modify this if rotation based on direction is needed

        // Initialize logic
        projectile.Initialize(config, source, direction);
    }
}