using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The central hub for an Enemy entity.
/// Manages components, implements IActor/ILootSource, and handles Identity.
/// </summary>
public class Enemy : MonoBehaviour, IActor, ILootSource
{
    // Configuration
    private EnemyDataSO config;

    // Sub-Components
    private HealthComponent healthComponent;
    private EnemyMovement movement;
    private EnemyWeapon weapon;
    private ScreenBoundsHandlerComponent bounds;
    private List<IGameComponent> gameComponents;

    // State
    private Rigidbody rb;

    // --- IActor Implementation ---
    public Transform GetTransform() => rb != null ? rb.transform : transform;
    public Rigidbody GetRigidbody() => rb;
    public Vector2 GetCurrentVelocity() => movement != null ? movement.GetVelocity() : Vector2.zero;
    public void SetCurrentVelocity(Vector2 velocity) { }

    public T GetAttachedComponent<T>() where T : IGameComponent
    {
        return gameComponents.OfType<T>().FirstOrDefault();
    }

    public LootTableSO GetLootTable() => config != null ? config.lootTable : null;

    // --- Initialization ---

    public void Initialize(EnemyDataSO data, IActor targetPlayer, BulletPool bulletPool = null)
    {
        config = data;

        // 1. Set Tag and Layer explicitly
        gameObject.tag = "Enemy";
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer > -1) gameObject.layer = enemyLayer;
        else Debug.LogWarning("Layer 'Enemy' does not exist in Project Settings!");

        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        gameComponents = new List<IGameComponent>();

        // Setup Components (Health, Movement, Weapon, Bounds)
        healthComponent = GetOrAddComponent<HealthComponent>();
        healthComponent.Initialize(this);
        healthComponent.ResetHealth();
        healthComponent.OnDeath -= OnDeath;
        healthComponent.OnDeath += OnDeath;
        gameComponents.Add(healthComponent);

        if (config.movementPattern != null || config.rotationPattern != null)
        {
            movement = GetOrAddComponent<EnemyMovement>();
            gameComponents.Add(movement);
            movement.Initialize(this);
            movement.Setup(config.movementPattern, config.rotationPattern, targetPlayer, config.moveSpeed);
        }

        if (config.attackPattern != null)
        {
            weapon = GetOrAddComponent<EnemyWeapon>();
            gameComponents.Add(weapon);
            weapon.Initialize(this);
            weapon.Setup(config.attackPattern, config, config.fireRate, targetPlayer, bulletPool);
        }

        bounds = GetOrAddComponent<ScreenBoundsHandlerComponent>();
        bounds.Initialize(this);
        bounds.Configure(0.2f);
        gameComponents.Add(bounds);
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        T comp = GetComponent<T>();
        if (comp == null) comp = gameObject.AddComponent<T>();
        return comp;
    }

    private void OnDeath(GameObject obj)
    {
        Vector3 deathPos = rb != null ? rb.position : transform.position;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEnemyDeath(deathPos, this);
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (healthComponent != null) healthComponent.OnDeath -= OnDeath;
    }
}