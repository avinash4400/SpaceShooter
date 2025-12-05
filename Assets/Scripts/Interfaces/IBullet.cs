using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Interface for objects that define how a bullet is fired (Patterns).
/// </summary>
public interface IBullet
{
    /// <summary>
    /// Fires the bullet pattern.
    /// </summary>
    /// <param name="source">The actor firing the bullet.</param>
    /// <param name="muzzles">List of available muzzle definitions.</param>
    /// <param name="direction">The general fire direction.</param>
    /// <param name="config">The configuration data for the bullet.</param>
    /// <param name="pool">The object pool to retrieve projectiles from.</param>
    /// <param name="target">The target actor (optional, can be null).</param>
    void Fire(IActor source, List<MuzzleDefinition> muzzles, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool, IActor target);
}