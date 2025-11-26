using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Abstract base class for all projectiles.
/// Implements IDamageSource, IActor, and pooling/recycling logic.
/// Updated to support Rigidbody physics and explicit interface implementations.
/// </summary>
public abstract class BaseProjectile : MonoBehaviour, IDamageSource, IActor
{
    // --- IDamageSource Implementation ---
    public int DamageAmount { get; protected set; }
    public IActor SourceActor { get; protected set; }

    /// <summary>
    /// Creates the DamageInfo struct required by the IDamageHandler.
    /// Explicit implementation since IDamageSource no longer has a default implementation.
    /// </summary>
    public DamageInfo CreateDamageInfo()
    {
        return new DamageInfo(DamageAmount, SourceActor);
    }

    // Used by the PlayerShooting to recycle this instance
    public event Action<BaseProjectile> OnProjectileExpired;

    // --- State & Config ---
    protected BulletTypeSO config;
    protected float moveSpeed;
    protected float lifeTimer;
    protected Vector3 fireDirection;

    // Physics component
    protected Rigidbody rb;

    // Performance optimization: Cache the camera
    private Camera mainCamera;

    public BulletTypeSO Config => config; // Expose config for Pooler identification

    // --- IActor Implementation ---

    public Transform GetTransform() => transform;

    public Rigidbody GetRigidbody() => rb;

    public Vector2 GetCurrentVelocity()
    {
        // For standard projectiles, velocity is constant direction * speed
        return fireDirection * moveSpeed;
    }

    public void SetCurrentVelocity(Vector2 velocity)
    {
        // Allow external systems to modify trajectory if needed
        moveSpeed = velocity.magnitude;
        if (moveSpeed > 0.001f)
        {
            fireDirection = velocity.normalized;
        }
    }

    /// <summary>
    /// Initializes the projectile.
    /// </summary>
    public virtual void Initialize(BulletTypeSO bulletConfig, IActor source, Vector3 direction)
    {
        config = bulletConfig;

        DamageAmount = config.damage;
        SourceActor = source;
        moveSpeed = config.speed;
        lifeTimer = config.lifetime;
        fireDirection = direction.normalized;

        // Ensure Rigidbody exists and is configured for projectile physics
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
        }

        // Cache camera reference if needed (optimization)
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Configure Rigidbody for kinematic movement (controlled via script, not forces)
        rb.useGravity = false;
        rb.isKinematic = true;
        // Ensure collision detection works for fast moving objects if needed, 
        // though ContinuousSpeculative is safer for kinematics.
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        gameObject.SetActive(true);
        StartCoroutine(LifeCountdownCoroutine());
    }

    protected virtual void Expire()
    {
        StopAllCoroutines();
        OnProjectileExpired?.Invoke(this);
    }

    /// <summary>
    /// Handles the movement update. Derived classes MUST implement this.
    /// </summary>
    protected abstract void Move();

    /// <summary>
    /// Using FixedUpdate for consistent physics movement.
    /// </summary>
    void FixedUpdate()
    {
        Move();
        CheckOutOfBounds();
    }

    /// <summary>
    /// Checks if the projectile has left the viewable screen area.
    /// </summary>
    private void CheckOutOfBounds()
    {
        if (mainCamera == null) return;

        // Convert world position to viewport position (0,0 is bottom-left, 1,1 is top-right)
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // Check if out of bounds with a small buffer (0.1) to ensure it's fully off-screen before recycling.
        // This prevents bullets from "popping" out of existence right at the edge.
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