using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Abstract base class for all projectiles.
/// </summary>
public abstract class BaseProjectile : MonoBehaviour, IDamageSource, IActor
{
    public int DamageAmount { get; protected set; }
    public IActor SourceActor { get; protected set; }

    public DamageInfo CreateDamageInfo()
    {
        return new DamageInfo(DamageAmount, SourceActor);
    }

    public event Action<BaseProjectile> OnProjectileExpired;

    protected BulletTypeSO config;
    protected float moveSpeed;
    protected float lifeTimer;
    protected Vector3 fireDirection;

    protected IActor target;

    protected Rigidbody rb;
    private Camera mainCamera;

    public BulletTypeSO Config => config;

    public Transform GetTransform() => transform;
    public Rigidbody GetRigidbody() => rb;
    public Vector2 GetCurrentVelocity() => fireDirection * moveSpeed;

    public void SetCurrentVelocity(Vector2 velocity)
    {
        moveSpeed = velocity.magnitude;
        if (moveSpeed > 0.001f) fireDirection = velocity.normalized;
    }

    public T GetAttachedComponent<T>() where T : IGameComponent
    {
        return GetComponent<T>();
    }

    /// <summary>
    /// Initializes the projectile.
    /// </summary>
    public virtual void Initialize(
        BulletTypeSO bulletConfig,
        IActor source,
        Vector3 direction,
        float speedMultiplier = 1f,
        IActor target = null 
    )
    {
        config = bulletConfig;
        DamageAmount = config.damage;
        SourceActor = source;
        this.target = target; 

        moveSpeed = config.speed * speedMultiplier;
        lifeTimer = config.lifetime;
        fireDirection = direction.normalized;

        AssignLayerBasedOnSource(source);

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) mainCamera = FindAnyObjectByType<Camera>();
        }

        gameObject.SetActive(true);
        StartCoroutine(LifeCountdownCoroutine());
    }

    private void AssignLayerBasedOnSource(IActor source)
    {
        int sourceLayer = source.GetTransform().gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (sourceLayer == playerLayer)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
        }
        else if (sourceLayer == enemyLayer)
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        }
    }

    protected virtual void Expire()
    {
        StopAllCoroutines();
        OnProjectileExpired?.Invoke(this);
    }

    protected abstract void Move();

    void FixedUpdate()
    {
        Move();
        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        if (mainCamera == null) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

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