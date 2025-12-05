using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstract base class for bullet patterns as ScriptableObjects.
/// </summary>
public abstract class BulletPatternSO : ScriptableObject, IBullet
{
    public abstract void Fire(IActor source, List<MuzzleDefinition> muzzles, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool, IActor target);

    /// <summary>
    /// Helper to get and initialize a projectile from the pool.
    /// </summary>
    protected void SpawnProjectile(ObjectPool<BaseProjectile> pool, BulletTypeSO config, IActor source, Vector3 position, Vector3 direction, IActor target)
    {
        BaseProjectile projectile = pool.Get();

        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity;

        projectile.Initialize(config, source, direction, 1f, target);
    }
}