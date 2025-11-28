using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Abstract base class for all projectiles.
/// Implements IDamageSource, IActor, and pooling/recycling logic.
/// Uses Transform-based movement (non-physics).
/// </summary>
public abstract class BaseProjectile : MonoBehaviour, IDamageSource, IActor
{
    // --- IDamageSource Implementation ---
    public int DamageAmount { get; protected set; }
    public IActor SourceActor { get; protected set; }

    public DamageInfo CreateDamageInfo()
    {
        return new DamageInfo(DamageAmount, SourceActor);
    }

    public event Action<BaseProjectile> OnProjectileExpired;

    // --- State & Config ---
    protected BulletTypeSO config;
    protected float moveSpeed;
    protected float lifeTimer;
    protected Vector3 fireDirection;

    // Cache camera for bounds check
    private Camera mainCamera;

    public BulletTypeSO Config => config;

    // --- IActor Implementation ---
    public Transform GetTransform() => transform;
    public Rigidbody GetRigidbody() => null; // No RB for simple projectiles
    public Vector2 GetCurrentVelocity() => fireDirection * moveSpeed;
    public void SetCurrentVelocity(Vector2 velocity)
    {
        moveSpeed = velocity.magnitude;
        if (moveSpeed > 0.001f) fireDirection = velocity.normalized;
    }

    public virtual void Initialize(BulletTypeSO bulletConfig, IActor source, Vector3 direction)
    {
        config = bulletConfig;
        DamageAmount = config.damage;
        SourceActor = source;
        moveSpeed = config.speed;
        lifeTimer = config.lifetime;
        fireDirection = direction.normalized;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) mainCamera = FindAnyObjectByType<Camera>();
        }

        gameObject.SetActive(true);
        StartCoroutine(LifeCountdownCoroutine());
    }

    public T GetAttachedComponent<T>() where T : IGameComponent
    {
        return default;
    }

    protected virtual void Expire()
    {
        StopAllCoroutines();
        OnProjectileExpired?.Invoke(this);
    }

    protected abstract void Move();

    void Update()
    {
        Move();
        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        if (mainCamera == null) return;
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // Buffer of 0.1 to ensure full exit
        if (viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
        {
            Expire();
        }
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