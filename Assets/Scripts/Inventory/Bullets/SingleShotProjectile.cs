using UnityEngine;

/// <summary>
/// Implements a standard, linear moving projectile.
/// </summary>
public class SingleShotProjectile : BaseProjectile
{
    protected override void Move()
    {
        Vector3 nextPosition = rb.position + (fireDirection * moveSpeed * Time.fixedDeltaTime);

        nextPosition.z = 0f;

        rb.MovePosition(nextPosition);
    }

    protected override void HandleCollision(Collider other)
    {
        IDamageHandler damageHandler = other.GetComponentInParent<IDamageHandler>();

        if (damageHandler != null)
        {
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