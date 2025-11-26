using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the player's current bullet type selection, limited ammo counts,
/// AND handles the actual spawning/pooling of projectiles (Weapon System).
/// </summary>
public class BulletInventory : MonoBehaviour, IGameComponent
{
    [Header("Configuration")]
    [SerializeField] private BulletFactory bulletFactory;
    [SerializeField] private int initialPoolSize = 20;

    [Header("Starting Ammo")]
    [SerializeField] private int doubleShotStartAmmo = 50;
    [SerializeField] private int tripleShotStartAmmo = 30;

    // --- Events ---
    public static event Action<BulletTypeSO, int> OnAmmoCountChanged;
    public static event Action<BulletTypeSO> OnBulletSelected;

    // --- State ---
    private IActor actor;

    // Pools mapped by BulletType
    private Dictionary<BulletType, ObjectPool<BaseProjectile>> bulletPools;

    // Key: BulletType Enum, Value: Ammo Count
    private Dictionary<BulletType, int> limitedAmmoCounts = new Dictionary<BulletType, int>();

    public BulletTypeSO SelectedBullet { get; private set; }
    private List<BulletTypeSO> availableBulletTypes = new List<BulletTypeSO>();

    public void Initialize(IActor actor)
    {
        this.actor = actor;

        if (bulletFactory == null)
        {
            Debug.LogError("[BulletInventory] BulletFactory not assigned!");
            return;
        }

        InitializeAmmo();
        InitializePools();
    }

    private void InitializePools()
    {
        bulletPools = new Dictionary<BulletType, ObjectPool<BaseProjectile>>();

        foreach (var config in bulletFactory.GetAllTypes())
        {
            if (config.projectilePrefab == null) continue;

            ObjectPool<BaseProjectile> pool = ObjectPooler.CreatePool<BaseProjectile>(
                config.projectilePrefab,
                initialPoolSize
            );

            bulletPools.Add(config.type, pool);
        }
    }

    private void InitializeAmmo()
    {
        BulletTypeSO[] allTypes = bulletFactory.GetAllTypes();
        availableBulletTypes.Clear();

        BulletTypeSO defaultType = null;

        foreach (var typeSO in allTypes)
        {
            availableBulletTypes.Add(typeSO);

            if (typeSO.type == BulletType.SingleShot)
            {
                defaultType = typeSO;
            }
            else if (typeSO.hasLimitedAmmo)
            {
                int startAmount = 0;
                switch (typeSO.type)
                {
                    case BulletType.DoubleShot: startAmount = doubleShotStartAmmo; break;
                    case BulletType.TripleShot: startAmount = tripleShotStartAmmo; break;
                }

                limitedAmmoCounts[typeSO.type] = startAmount;
                OnAmmoCountChanged?.Invoke(typeSO, startAmount);
            }
        }

        if (defaultType != null) SelectBullet(defaultType);
        else if (availableBulletTypes.Count > 0) SelectBullet(availableBulletTypes[0]);

        availableBulletTypes = availableBulletTypes.OrderBy(b => b.type).ToList();
    }

    /// <summary>
    /// Attempts to fire the currently selected bullet.
    /// Handles ammo check, consumption, and spawning.
    /// </summary>
    public void AttemptFire(Vector3 spawnPosition, Vector3 fireDirection, IActor sourceActor)
    {
        if (SelectedBullet == null) return;

        // 1. Consume Ammo
        if (!ConsumeAmmo(SelectedBullet)) return;

        // 2. Spawn from Pool
        SpawnBullet(SelectedBullet, spawnPosition, fireDirection, sourceActor);
    }

    private void SpawnBullet(BulletTypeSO bulletConfig, Vector3 position, Vector3 direction, IActor source)
    {
        if (bulletPools == null || !bulletPools.ContainsKey(bulletConfig.type)) return;

        ObjectPool<BaseProjectile> pool = bulletPools[bulletConfig.type];
        BaseProjectile projectile = pool.Get();

        // Subscribe to return event
        projectile.OnProjectileExpired -= ReturnToPool;
        projectile.OnProjectileExpired += ReturnToPool;

        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity;

        projectile.Initialize(bulletConfig, source, direction);
    }

    private void ReturnToPool(BaseProjectile projectile)
    {
        if (projectile.Config == null) return;

        BulletType type = projectile.Config.type;
        if (bulletPools.ContainsKey(type))
        {
            bulletPools[type].Return(projectile);
        }
        else
        {
            Destroy(projectile.gameObject);
        }
    }

    public void SwitchBullet()
    {
        if (availableBulletTypes.Count <= 1) return;

        int currentIndex = availableBulletTypes.IndexOf(SelectedBullet);
        int nextIndex = (currentIndex + 1) % availableBulletTypes.Count;

        for (int i = 0; i < availableBulletTypes.Count; i++)
        {
            BulletTypeSO nextType = availableBulletTypes[nextIndex];
            if (SelectBullet(nextType)) return;
            nextIndex = (nextIndex + 1) % availableBulletTypes.Count;
        }
    }

    public bool SelectBullet(BulletTypeSO bulletType)
    {
        if (bulletType == null) return false;

        if (!bulletType.hasLimitedAmmo || (limitedAmmoCounts.ContainsKey(bulletType.type) && limitedAmmoCounts[bulletType.type] > 0))
        {
            SelectedBullet = bulletType;
            OnBulletSelected?.Invoke(SelectedBullet);

            int currentAmmo = limitedAmmoCounts.ContainsKey(bulletType.type) ? limitedAmmoCounts[bulletType.type] : -1;
            OnAmmoCountChanged?.Invoke(bulletType, currentAmmo);

            return true;
        }
        return false;
    }

    public bool ConsumeAmmo(BulletTypeSO bulletType)
    {
        if (bulletType == null || !bulletType.hasLimitedAmmo) return true;

        if (limitedAmmoCounts.ContainsKey(bulletType.type))
        {
            if (limitedAmmoCounts[bulletType.type] <= 0) return false;

            limitedAmmoCounts[bulletType.type]--;
            int currentAmmo = limitedAmmoCounts[bulletType.type];

            OnAmmoCountChanged?.Invoke(bulletType, currentAmmo);

            if (currentAmmo <= 0 && SelectedBullet == bulletType)
            {
                var defaultType = availableBulletTypes.FirstOrDefault(b => b.type == BulletType.SingleShot);
                if (defaultType != null) SelectBullet(defaultType);
            }
            return true;
        }
        return false;
    }
}