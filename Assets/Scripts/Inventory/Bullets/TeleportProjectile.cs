using UnityEngine;

public class TeleportProjectile : BaseProjectile
{
    [Header("Beacon Settings")]
    [Tooltip("How long the beacon stays active before fizzling out.")]
    [SerializeField] private float beaconLifetime = 5f;

    // State
    private float timeSinceSpawn;

    protected override void Move()
    {
        // Calculate next position using Rigidbody physics (matching SingleShotProjectile)
        Vector3 nextPosition = rb.position + (fireDirection * moveSpeed * Time.fixedDeltaTime);

        // STRICTLY enforce Z=0
        nextPosition.z = 0f;

        rb.MovePosition(nextPosition);
    }

    protected override void HandleCollision(Collider other)
    {
        // Intentionally empty.
        // 1. Ignores Enemies (Ghost behavior).
        // 2. Ignores Walls (Removed stickiness).
        // The beacon will float through geometry until the player teleports to it or it expires.
    }
}