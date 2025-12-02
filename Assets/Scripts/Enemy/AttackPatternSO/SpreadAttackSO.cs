using UnityEngine;

/// <summary>
/// Fires multiple bullets in a fan/arc pattern.
/// Great for Bosses or Shotgun-style enemies.
/// </summary>
[CreateAssetMenu(fileName = "SpreadAttack", menuName = "Game/Enemy/Attack/Spread")]
public class SpreadAttackSO : EnemyAttackSO
{
    [Header("Spread Settings")]
    [Tooltip("Number of bullets to fire in one volley.")]
    [Min(1)]
    [SerializeField] private int projectileCount = 5;

    [Tooltip("Total angle of the spread in degrees (e.g. 45).")]
    [SerializeField] private float spreadAngle = 45f;

    public override float ExecuteAttack(
        IActor attacker,
        EnemyWeapon weapon,
        IActor target,
        EnemyDataSO data)
    {
        if (weapon == null) return attackCooldown;

        ObjectPool<BaseProjectile> pool = GetPool();
        if (pool == null) return attackCooldown;

        Transform muzzleTransform = weapon.GetMuzzle(muzzleType);
        if (muzzleTransform == null) return attackCooldown;

        // 1. Calculate Spacing
        // If count is 1, spread is 0. Otherwise divide angle by spaces between bullets.
        float angleStep = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;

        // Start from the leftmost angle (negative half of total spread)
        float currentAngle = -spreadAngle / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            // 2. Calculate Rotation
            // We rotate around Z axis for 2D, relative to the muzzle's facing direction
            Quaternion rotation = muzzleTransform.rotation * Quaternion.Euler(0, 0, currentAngle);
            Vector3 fireDirection = rotation * Vector3.up;

            // 3. Spawn from Pool
            BaseProjectile bullet = pool.Get();

            // Force Z=0 for 2D gameplay
            Vector3 spawnPos = muzzleTransform.position;
            spawnPos.z = 0f;

            bullet.transform.position = spawnPos;
            bullet.transform.rotation = rotation;

            // 4. Initialize
            // Uses the speedMultiplier defined in the EnemyAttackSO base class
            bullet.Initialize(bulletType, attacker, fireDirection, speedMultiplier);

            // 5. Increment Angle for next bullet
            currentAngle += angleStep;
        }

        return attackCooldown;
    }
}