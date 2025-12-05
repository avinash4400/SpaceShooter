using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour, IActor, ILootSource
{
    private EnemyDataSO config;
    private HealthComponent healthComponent;
    private EnemyMovement movement;
    private EnemyWeapon weapon;
    private ScreenBoundsHandlerComponent bounds;
    private CollisionDamageComponent collisionDamage;
    private DeathVisuals deathVisuals;
    private BossHitVisuals bossHitVisuals;
    private List<IGameComponent> gameComponents;
    private Rigidbody rb;
    private bool isDead = false;

    public Transform GetTransform() => (rb != null) ? rb.transform : transform;
    public Rigidbody GetRigidbody() => rb;
    public Vector2 GetCurrentVelocity() => movement != null ? movement.GetVelocity() : Vector2.zero;
    public void SetCurrentVelocity(Vector2 velocity) { }
    public T GetAttachedComponent<T>() where T : IGameComponent => gameComponents.OfType<T>().FirstOrDefault();
    public LootTableSO GetLootTable() => config != null ? config.GetLootTable() : null;

    void Start()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEnemySpawned(this);
        }
    }

    public void Initialize(EnemyDataSO data, IActor targetPlayer, BulletPool bulletPool = null)
    {
        config = data;
        isDead = false;
        gameObject.tag = "Enemy";
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer > -1) gameObject.layer = enemyLayer;

        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.detectCollisions = true;

        gameComponents = new List<IGameComponent>();

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
            movement.enabled = true;
        }

        if (config.attackPattern != null)
        {
            weapon = GetOrAddComponent<EnemyWeapon>();
            gameComponents.Add(weapon);
            weapon.Initialize(this);
            weapon.Setup(config.attackPattern, config, targetPlayer);
            weapon.enabled = true;
        }

        deathVisuals = GetOrAddComponent<DeathVisuals>();
        deathVisuals.Initialize(this);
        gameComponents.Add(deathVisuals);

        bossHitVisuals = GetComponentInChildren<BossHitVisuals>();
        if (bossHitVisuals != null)
        {
            bossHitVisuals.Initialize(this);
            gameComponents.Add(bossHitVisuals);
        }

        bounds = GetOrAddComponent<ScreenBoundsHandlerComponent>();
        bounds.Initialize(this);
        bounds.Configure(0.2f);
        gameComponents.Add(bounds);

        collisionDamage = GetOrAddComponent<CollisionDamageComponent>();
        collisionDamage.Initialize(this);
        gameComponents.Add(collisionDamage);
        collisionDamage.enabled = true;
    }

    public void OverrideMovement(EnemyMovementSO move, EnemyRotationSO rot)
    {
        if (movement != null) movement.UpdateStrategies(move, rot);
    }

    public void OverrideAttack(EnemyAttackSO attack)
    {
        if (weapon != null) weapon.UpdateStrategy(attack);
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        T comp = GetComponentInChildren<T>();
        if (comp == null) comp = GetComponentInParent<T>();
        if (comp == null) comp = gameObject.AddComponent<T>();
        return comp;
    }

    private void OnDeath(GameObject obj)
    {
        if (isDead) return;
        isDead = true;

        Vector3 deathPos = rb != null ? rb.position : transform.position;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEnemyDeath(deathPos, this);
            if (config != null)
            {
                EventManager.Instance.TriggerAddScore(config.scoreValue);

                EventManager.Instance.TriggerExplosion(deathPos, config.deathSound);
            }
        }

        DisableGameplayComponents();

        if (deathVisuals != null)
        {
            deathVisuals.StartDeathEffect(SafeDestroy);
        }
        else
        {
            SafeDestroy();
        }
    }

    private void DisableGameplayComponents()
    {
        if (movement != null) movement.enabled = false;
        if (weapon != null) weapon.enabled = false;
        if (collisionDamage != null) collisionDamage.enabled = false;
        if (bounds != null) bounds.enabled = false;

        if (rb != null) rb.detectCollisions = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
    }

    private void SafeDestroy()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (healthComponent != null) healthComponent.OnDeath -= OnDeath;

        if (EventManager.Instance != null && !isDead)
        {
            EventManager.Instance.TriggerEnemyDespawned(this);
        }

        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEnemyDespawned(this);
        }
    }
}