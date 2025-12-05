using UnityEngine;

public class TeleportProjectile : BaseProjectile
{
    [Header("Beacon Settings")]
    [Tooltip("How long the beacon stays active before fizzling out.")]
    [SerializeField] private float beaconLifetime = 5f;

    private float timeSinceSpawn;

    protected override void Move()
    {
        Vector3 nextPosition = rb.position + (fireDirection * moveSpeed * Time.fixedDeltaTime);

        nextPosition.z = 0f;

        rb.MovePosition(nextPosition);
    }

    protected override void HandleCollision(Collider other)
    {
    }
}