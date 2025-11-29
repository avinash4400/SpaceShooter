using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized manager for Bullet Pools.
/// Allows multiple entities (Enemies, Player) to share pools for the same bullet type.
/// Implements Lazy Initialization.
/// </summary>
public class BulletManager : Singleton<BulletManager>
{
    // Dictionary mapping specific BulletTypeSO to its Object Pool
    private Dictionary<BulletTypeSO, BulletPool> pools = new Dictionary<BulletTypeSO, BulletPool>();

    [Header("Configuration")]
    [SerializeField] private int defaultPoolSize = 20;

    /// <summary>
    /// Retrieves (or creates) a BulletPool for the specified bullet configuration.
    /// </summary>
    public BulletPool GetPool(BulletTypeSO bulletType)
    {
        if (bulletType == null || bulletType.projectilePrefab == null)
        {
            Debug.LogWarning("[BulletManager] Requesting pool for null config or missing prefab.");
            return null;
        }

        if (pools.ContainsKey(bulletType))
        {
            return pools[bulletType];
        }

        // Lazy Initialization: Create the pool if it doesn't exist
        GameObject poolGroup = new GameObject($"Pool_{bulletType.name}");
        poolGroup.transform.SetParent(transform);

        BulletPool newPool = new BulletPool(
            bulletType.projectilePrefab,
            defaultPoolSize,
            poolGroup.transform
        );

        pools.Add(bulletType, newPool);
        return newPool;
    }
}