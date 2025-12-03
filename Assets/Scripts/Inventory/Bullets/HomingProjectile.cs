using UnityEngine;

/// <summary>
/// A projectile that steers towards its target until a certain range, 
/// then locks onto the position and flies straight to detonate.
/// Damages all entities in an area upon impact.
/// </summary>
public class HomingProjectile : BaseProjectile
{
    [Header("Homing Settings")]
    [Tooltip("How fast the missile turns towards the target (deg/sec).")]
    [SerializeField] private float turnSpeed = 200f;

    [Tooltip("Distance at which the missile stops tracking and locks onto the current position.")]
    [SerializeField] private float lockOnDistance = 3f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 2.0f;
    [SerializeField] private int damage = 1; // Note: Overrides BaseProjectile damage usually, or acts as splash damage

    // State
    private bool isLocked = false;
    private Vector3 lockedPosition;

    public override void Initialize(BulletTypeSO bulletConfig, IActor source, Vector3 direction, float speedMultiplier = 1f, IActor target = null)
    {
        base.Initialize(bulletConfig, source, direction, speedMultiplier, target);
        isLocked = false;

        // If no target provided, we can't home, so we just fly straight (BaseProjectile behavior)
        // or we could try to find one. For now, we rely on the passed target.
    }

    protected override void Move()
    {
        float dt = Time.fixedDeltaTime;
        Vector3 currentPos = rb.position;

        if (isLocked)
        {
            // PHASE 3: Terminal Guidance (Fly straight to locked spot)
            Vector3 dirToLock = (lockedPosition - currentPos).normalized;

            // Move
            Vector3 nextPos = currentPos + (dirToLock * moveSpeed * dt);
            nextPos.z = 0f;
            rb.MovePosition(nextPos);

            // Rotation (Look at target)
            float angle = Mathf.Atan2(dirToLock.y, dirToLock.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Check Arrival
            if (Vector3.Distance(currentPos, lockedPosition) < 0.5f)
            {
                Explode();
            }
        }
        else
        {
            // PHASE 1: Tracking
            if (target != null)
            {
                Vector3 targetPos = target.GetTransform().position;
                targetPos.z = 0f;

                // Check Distance for Phase 2 Trigger
                float dist = Vector3.Distance(currentPos, targetPos);
                if (dist <= lockOnDistance)
                {
                    isLocked = true;
                    lockedPosition = targetPos;
                    return; // Next frame will handle movement to lock
                }

                // Steering Logic
                Vector3 directionToTarget = (targetPos - currentPos).normalized;

                // Rotate smoothly towards target
                // Calculate z-angle
                float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);

                rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * dt);

                // Update movement direction based on new rotation
                fireDirection = rb.transform.up;
            }

            // Move Forward (relative to current rotation)
            Vector3 nextPos = currentPos + (fireDirection * moveSpeed * dt);
            nextPos.z = 0f;
            rb.MovePosition(nextPos);
        }
    }

    protected override void HandleCollision(Collider other)
    {
        // Safety check to avoid hitting self/source
        if (SourceActor != null && other.gameObject == SourceActor.GetTransform().gameObject) return;

        // Check if we hit something valid (Enemy/Player/Wall)
        // We let the Explosion logic handle the damage dealing
        // But we need to ensure we don't explode on triggers like "Zones" unless they are damageable

        // Simple filter: If it's a damage handler or a solid wall, explode.
        // Or simply explode on anything that isn't the shooter.

        Explode();
    }

    private void Explode()
    {
        // 1. Visuals (Spawn explosion prefab if we had one in config, or handle via Event)
        // Debug.Log("Boom!");

        // 2. Area of Effect Damage
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            // Friendly Fire Logic:
            // If Source is Player, don't damage Player layer
            // If Source is Enemy, don't damage Enemy layer

            bool hitIsSource = false;
            // Check if SourceActor is still valid (not destroyed)
            if (SourceActor != null && !SourceActor.Equals(null))
            {
                try
                {
                    // Safe access check
                    if (SourceActor.GetTransform() != null && hit.gameObject == SourceActor.GetTransform().gameObject)
                    {
                        hitIsSource = true;
                    }
                }
                catch
                {
                    // Source was destroyed, so we definitely didn't hit it
                    hitIsSource = false;
                }
            }

            if (hitIsSource) continue;

            // Check Layer compatibility
            // Quick check: If I am PlayerBullet, I shouldn't hurt Player
            if (gameObject.layer == LayerMask.NameToLayer("PlayerBullet") && hit.CompareTag("Player")) continue;
            if (gameObject.layer == LayerMask.NameToLayer("EnemyBullet") && hit.CompareTag("Enemy")) continue;

            IDamageHandler handler = hit.GetComponentInParent<IDamageHandler>();
            if (handler != null)
            {
                // Use the configured damage
                handler.HandleDamage(CreateDamageInfo());
            }
        }

        // 3. Destroy/Recycle
        Expire();
    }
}