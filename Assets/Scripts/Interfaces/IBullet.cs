using UnityEngine;

/// <summary>
/// Strategy interface for firing logic.
/// Defines HOW projectiles are spawned (Pattern), independent of WHAT is spawned (Prefab).
/// </summary>
public interface IBullet
{
    /// <summary>
    /// Executes the firing logic.
    /// </summary>
    /// <param name="source">The actor firing the shot (for damage attribution).</param>
    /// <param name="origin">Muzzle position.</param>
    /// <param name="direction">Aiming direction.</param>
    /// <param name="config">Configuration data (speed, damage, count).</param>
    /// <param name="pool">The object pool to retrieve projectiles from.</param>
    void Fire(IActor source, Vector3 origin, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool);
}