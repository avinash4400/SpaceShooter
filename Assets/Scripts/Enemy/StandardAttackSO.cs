using UnityEngine;

/// <summary>
/// Concrete Attack Strategy: Standard Forward Fire.
/// Fires a bullet in the direction the enemy is currently facing (Transform.up).
/// Ignores the target position directly, relying on the Rotation Strategy to aim.
/// </summary>
[CreateAssetMenu(fileName = "StandardAttack", menuName = "Game/Enemy/Attack/Standard")]
public class StandardAttackSO : EnemyAttackSO
{
    [Header("Settings")]
    [Tooltip("Offset from the center of the enemy where the bullet spawns.")]
    [SerializeField] private Vector3 muzzleOffset = new Vector3(0, -0.5f, 0);

    public override void ExecuteAttack(IActor attacker, IActor target, EnemyDataSO data, ObjectPool<BaseProjectile> bulletPool)
    {
        if (bulletPool == null) return;

        // 1. Get Orientation
        Transform t = attacker.GetTransform();

        // 2. Calculate Spawn Position (Offset relative to rotation)
        Vector3 spawnPos = t.position + (t.rotation * muzzleOffset);

        // 3. Determine Direction
        // STRICTLY use the actor's facing direction (Up vector for 2D sprites).
        // This ensures the bullet goes where the enemy is looking, not magically at the player.
        Vector3 fireDirection = t.up;

        // 4. Spawn & Initialize
        BaseProjectile bullet = bulletPool.Get();

        bullet.transform.position = spawnPos;
        bullet.transform.rotation = t.rotation; // Align bullet sprite with enemy rotation

        bullet.Initialize(data.bulletType, attacker, fireDirection);
    }
}