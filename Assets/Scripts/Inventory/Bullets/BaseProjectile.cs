using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Abstract base class for all projectiles.
/// Implements IDamageSource, IActor, and pooling/recycling logic.
/// Automatically handles Layer assignment and Out-of-Bounds recycling.
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

    // Used by the BulletPool to recycle this instance
    public event Action<BaseProjectile> OnProjectileExpired;

    // --- State & Config ---
    protected BulletTypeSO config;
    protected float moveSpeed;
    protected float lifeTimer;
    protected Vector3 fireDirection;

    // Physics & Rendering
    protected Rigidbody rb;
    private Camera mainCamera;

    public BulletTypeSO Config => config;

    // --- IActor Implementation ---
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
    /// Initializes the projectile, sets layers, and prepares for movement.
    /// </summary>
    public virtual void Initialize(BulletTypeSO bulletConfig, IActor source, Vector3 direction, float speedMultiplier = 1f)
    {
        config = bulletConfig;
        DamageAmount = config.damage;
        SourceActor = source;
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

        // Cache camera for bounds checking
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

    /// <summary>
    /// Checks if the projectile has left the viewport.
    /// Recycles it if it goes too far off-screen.
    /// </summary>
    private void CheckOutOfBounds()
    {
        if (mainCamera == null) return;

        // Convert world position to viewport (0,0 is bottom-left, 1,1 is top-right)
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // Check if out of bounds with a buffer (0.1 = 10% screen width)
        // This ensures the bullet is fully off-screen before disappearing.
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