using UnityEngine;

/// <summary>
/// Implements a standard, linear moving projectile.
/// Moves using Rigidbody physics instead of Transform modification.
/// </summary>
public class SingleShotProjectile : BaseProjectile
{
    protected override void Move()
    {
        // Calculate the next position based on velocity and fixed delta time
        Vector3 nextPosition = rb.position + (fireDirection * moveSpeed * Time.fixedDeltaTime);

        // STRICTLY enforce Z=0 to prevent drifting off the gameplay plane
        nextPosition.z = 0f;

        rb.MovePosition(nextPosition);
    }

    protected override void HandleCollision(Collider other)
    {
        IDamageHandler damageHandler = other.GetComponentInParent<IDamageHandler>();

        if (damageHandler != null)
        {
            // Friendly Fire Check using LAYERS
            int myLayer = gameObject.layer;
            int targetLayer = other.gameObject.layer;

            bool isPlayerBullet = myLayer == LayerMask.NameToLayer("PlayerBullet");
            bool isEnemyBullet = myLayer == LayerMask.NameToLayer("EnemyBullet");
            bool isTargetEnemy = targetLayer == LayerMask.NameToLayer("Enemy");
            bool isTargetPlayer = targetLayer == LayerMask.NameToLayer("Player");

            if ((isPlayerBullet && isTargetEnemy) || (isEnemyBullet && isTargetPlayer))
            {
                damageHandler.HandleDamage(CreateDamageInfo());
                Expire();
            }
        }
    }
}