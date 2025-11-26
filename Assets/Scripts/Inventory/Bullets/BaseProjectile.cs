using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Abstract base class for all projectiles.
/// Implements IDamageSource and pooling/recycling logic.
/// </summary>
public abstract class BaseProjectile : MonoBehaviour, IDamageSource
{
    // --- IDamageSource Implementation ---
    public int DamageAmount { get; protected set; }
    public GameObject SourceObject { get; protected set; }

    // Used by the PlayerShooting to recycle this instance
    public event Action<BaseProjectile> OnProjectileExpired;

    // --- State & Config ---
    protected BulletTypeSO config;
    protected float moveSpeed;
    protected float lifeTimer;
    protected Vector3 fireDirection;

    public BulletTypeSO Config => config; // Expose config for Pooler identification

    public virtual void Initialize(BulletTypeSO bulletConfig, GameObject source, Vector3 direction)
    {
        config = bulletConfig;

        DamageAmount = config.damage;
        SourceObject = source;
        moveSpeed = config.speed;
        lifeTimer = config.lifetime;
        fireDirection = direction.normalized;

        gameObject.SetActive(true);
        StartCoroutine(LifeCountdownCoroutine());
    }

    public virtual DamageInfo CreateDamageInfo()
    {
        return new DamageInfo(DamageAmount, SourceObject);
    }

    protected virtual void Expire()
    {
        // Don't disable here, let the pool handle it via the event listener
        // But we DO need to stop coroutines
        StopAllCoroutines();
        OnProjectileExpired?.Invoke(this);
    }

    protected abstract void Move();

    void Update()
    {
        Move();
    }

    protected abstract void HandleCollision(Collider other);

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private IEnumerator LifeCountdownCoroutine()
    {
        yield return new WaitForSeconds(config.lifetime);
        Expire();
    }
}