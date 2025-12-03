using UnityEngine;

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

        float angleStep = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;
        float currentAngle = -spreadAngle / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            Quaternion rotation = muzzleTransform.rotation * Quaternion.Euler(0, 0, currentAngle);
            Vector3 fireDirection = rotation * Vector3.up;

            BaseProjectile bullet = pool.Get();

            Vector3 spawnPos = muzzleTransform.position;
            spawnPos.z = 0f;

            bullet.transform.position = spawnPos;
            bullet.transform.rotation = rotation;

            // Pass the target
            bullet.Initialize(bulletType, attacker, fireDirection, speedMultiplier, target);

            currentAngle += angleStep;
        }

        return attackCooldown;
    }
}