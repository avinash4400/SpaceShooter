using UnityEngine;

/// <summary>
/// A projectile that steers towards its target until a certain range, 
/// then locks onto the position and flies straight to detonate.
/// Damages all entities in an area upon impact.
/// </summary>
public class HomingProjectile : BaseProjectile
{
    [Header("Homing Settings")]
    [SerializeField] private float turnSpeed = 200f;
    [SerializeField] private float lockOnDistance = 3f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 2.0f;
    [SerializeField] private int damage = 1;

    private bool isLocked = false;
    private Vector3 lockedPosition;

    public override void Initialize(BulletTypeSO bulletConfig, IActor source, Vector3 direction, float speedMultiplier = 1f, IActor target = null)
    {
        base.Initialize(bulletConfig, source, direction, speedMultiplier, target);
        isLocked = false;
    }

    protected override void Move()
    {
        float dt = Time.fixedDeltaTime;
        Vector3 currentPos = rb.position;

        if (isLocked)
        {
            Vector3 dirToLock = (lockedPosition - currentPos).normalized;
            Vector3 nextPos = currentPos + (dirToLock * moveSpeed * dt);
            nextPos.z = 0f;
            rb.MovePosition(nextPos);

            float angle = Mathf.Atan2(dirToLock.y, dirToLock.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Vector3.Distance(currentPos, lockedPosition) < 0.5f)
            {
                Explode();
            }
        }
        else
        {
            if (target != null)
            {
                Vector3 targetPos = target.GetTransform().position;
                targetPos.z = 0f;

                float dist = Vector3.Distance(currentPos, targetPos);
                if (dist <= lockOnDistance)
                {
                    isLocked = true;
                    lockedPosition = targetPos;
                    return;
                }

                Vector3 directionToTarget = (targetPos - currentPos).normalized;
                float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);

                rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * dt);
                fireDirection = rb.transform.up;
            }

            Vector3 nextPos = currentPos + (fireDirection * moveSpeed * dt);
            nextPos.z = 0f;
            rb.MovePosition(nextPos);
        }
    }

    protected override void HandleCollision(Collider other)
    {
        if (SourceActor != null && other.gameObject == SourceActor.GetTransform().gameObject) return;
        Explode();
    }

    private void Explode()
    {
        // NEW: Audio Event
        // Pass the hitSound configured in the BulletTypeSO
        if (EventManager.Instance != null && config != null)
        {
            EventManager.Instance.TriggerExplosion(transform.position, config.hitSound);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            bool hitIsSource = false;
            if (SourceActor != null && !SourceActor.Equals(null))
            {
                try
                {
                    if (SourceActor.GetTransform() != null && hit.gameObject == SourceActor.GetTransform().gameObject)
                    {
                        hitIsSource = true;
                    }
                }
                catch
                {
                    hitIsSource = false;
                }
            }

            if (hitIsSource) continue;

            if (gameObject.layer == LayerMask.NameToLayer("PlayerBullet") && hit.CompareTag("Player")) continue;
            if (gameObject.layer == LayerMask.NameToLayer("EnemyBullet") && hit.CompareTag("Enemy")) continue;

            IDamageHandler handler = hit.GetComponentInParent<IDamageHandler>();
            if (handler != null)
            {
                handler.HandleDamage(CreateDamageInfo());
            }
        }

        Expire();
    }
}