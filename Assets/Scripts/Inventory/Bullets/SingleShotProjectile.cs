using UnityEngine;

/// <summary>
/// Implements a standard, linear moving projectile (e.g., the Single Shot).
/// Moves using Rigidbody physics instead of Transform modification.
/// </summary>
public class SingleShotProjectile : BaseProjectile
{
    /// <summary>
    /// Standard linear movement update using Rigidbody.MovePosition.
    /// </summary>
    protected override void Move()
    {
        // Calculate the next position based on velocity and fixed delta time
        Vector3 nextPosition = rb.position + (fireDirection * moveSpeed * Time.fixedDeltaTime);

        // Apply physics-based movement
        rb.MovePosition(nextPosition);
    }

    /// <summary>
    /// Handles collision: check for damage handler and expire.
    /// </summary>
    protected override void HandleCollision(Collider other)
    {
        // Check if the hit object is a damage handler
        IDamageHandler damageHandler = other.GetComponentInParent<IDamageHandler>();

        if (damageHandler != null)
        {
            // Prevent friendly fire by checking if the source's tag matches the target's tag.
            if (SourceActor != null && other.CompareTag(SourceActor.GetTransform().tag))
            {
                return;
            }

            // Inflict damage using the IDamageSource interface
            damageHandler.HandleDamage(CreateDamageInfo());
            Debug.Log($"[SingleShotProjectile] Inflicted {DamageAmount} damage to {other.name}.");
            // Trigger hit effect/sound here

            Expire(); // Projectile is consumed on impact
        }
    }
}