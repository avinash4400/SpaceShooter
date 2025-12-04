using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BulletInventory : MonoBehaviour, IGameComponent
{
    // ... (Configuration fields)
    [Header("Configuration")]
    [SerializeField] private BulletFactory bulletFactory;
    [SerializeField] private int initialPoolSize = 20;

    [Header("Starting Ammo")]
    [SerializeField] private int doubleShotStartAmmo = 50;
    [SerializeField] private int tripleShotStartAmmo = 30;

    public static event Action<BulletTypeSO, int> OnAmmoCountChanged;
    public static event Action<BulletTypeSO> OnBulletSelected;

    private IActor actor;
    private Dictionary<BulletType, BulletPool> bulletPools;
    private Dictionary<BulletType, int> limitedAmmoCounts = new Dictionary<BulletType, int>();
    private List<BulletTypeSO> availableBulletTypes = new List<BulletTypeSO>();

    public BulletTypeSO SelectedBullet { get; private set; }

    public void Initialize(IActor actor)
    {
        this.actor = actor;
        if (bulletFactory == null) return;
        InitializeAmmo();
        InitializePools();
    }

    // ... (OnEnable/Disable, InitializePools, InitializeAmmo, SwitchBullet, SelectBullet, ConsumeAmmo unchanged) ...

    void OnEnable() { PlayerController.OnSwitchBulletInput += SwitchBullet; }
    void OnDisable() { PlayerController.OnSwitchBulletInput -= SwitchBullet; }

    private void InitializePools()
    {
        bulletPools = new Dictionary<BulletType, BulletPool>();
        GameObject poolRoot = new GameObject("BulletPools");
        poolRoot.transform.SetParent(transform.parent);

        foreach (var config in bulletFactory.GetAllTypes())
        {
            if (config.projectilePrefab == null) continue;
            BulletPool pool = new BulletPool(config.projectilePrefab, initialPoolSize, poolRoot.transform);
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
            if (typeSO.type == BulletType.SingleShot) defaultType = typeSO;
            else if (typeSO.hasLimitedAmmo)
            {
                int startAmount = (typeSO.type == BulletType.DoubleShot) ? doubleShotStartAmmo : tripleShotStartAmmo;
                limitedAmmoCounts[typeSO.type] = startAmount;
                OnAmmoCountChanged?.Invoke(typeSO, startAmount);
            }
        }
        if (defaultType != null) SelectBullet(defaultType);
        availableBulletTypes = availableBulletTypes.OrderBy(b => b.type).ToList();
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

    public void AttemptFire(Vector3 spawnPosition, Vector3 fireDirection, IActor sourceActor, IActor target = null)
    {
        if (SelectedBullet == null) return;
        if (!ConsumeAmmo(SelectedBullet)) return;

        if (bulletPools == null || !bulletPools.ContainsKey(SelectedBullet.type)) return;
        BulletPool pool = bulletPools[SelectedBullet.type];

        Vector3 flatSpawnPos = spawnPosition;
        flatSpawnPos.z = 0f;

        if (SelectedBullet.patternLogic != null)
        {
            SelectedBullet.patternLogic.Fire(sourceActor, flatSpawnPos, fireDirection, SelectedBullet, pool, target);
        }
        else
        {
            SpawnBullet(SelectedBullet, flatSpawnPos, fireDirection, sourceActor, target);
        }

        // NEW: Trigger Audio Event
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerPlayerFired(SelectedBullet);
        }
    }

    private void SpawnBullet(BulletTypeSO bulletConfig, Vector3 position, Vector3 direction, IActor source, IActor target)
    {
        if (bulletPools == null || !bulletPools.ContainsKey(bulletConfig.type)) return;
        BulletPool pool = bulletPools[bulletConfig.type];
        BaseProjectile projectile = pool.Get();

        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity;

        projectile.Initialize(bulletConfig, source, direction, 1f, target);
    }
}