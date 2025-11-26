using UnityEngine;

/// <summary>
/// Implements a standard, linear moving projectile (e.g., the Single Shot).
/// </summary>
public class SingleShotProjectile : BaseProjectile
{
    /// <summary>
    /// Standard linear movement update.
    /// </summary>
    protected override void Move()
    {
        transform.position += fireDirection * moveSpeed * Time.deltaTime;
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
            // IMPORTANT: Prevent friendly fire by checking if the source's tag matches the target's tag.
            // Tag is assumed to be set to "Player" or "Enemy".
            if (other.CompareTag(SourceObject.tag))
            {
                return; // Prevent friendly fire
            }

            // Inflict damage using the IDamageSource interface
            damageHandler.HandleDamage(CreateDamageInfo());

            // Trigger hit effect/sound here (to be implemented later)

            Expire(); // Projectile is consumed on impact
        }
    }
}