using UnityEngine;

/// <summary>
/// Implements a standard, linear moving projectile.
/// Uses Transform modification for movement.
/// </summary>
public class SingleShotProjectile : BaseProjectile
{
    protected override void Move()
    {
        transform.position += fireDirection * moveSpeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider other)
    {
        IDamageHandler damageHandler = other.GetComponentInParent<IDamageHandler>();

        if (damageHandler != null)
        {
            // Friendly fire check
            if (SourceActor != null && other.CompareTag(SourceActor.GetTransform().tag))
            {
                return;
            }
            
            damageHandler.HandleDamage(CreateDamageInfo());
            Expire(); 
        }
    }
}