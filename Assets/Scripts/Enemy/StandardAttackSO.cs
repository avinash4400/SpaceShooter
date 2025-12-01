using UnityEngine;

[CreateAssetMenu(fileName = "StandardAttack", menuName = "Game/Enemy/Attack/Standard")]
public class StandardAttackSO : EnemyAttackSO
{
    public override float ExecuteAttack(
        IActor attacker,
        EnemyWeapon weapon,
        IActor target,
        EnemyDataSO data,
        float speedMultiplier)
    {
        ObjectPool<BaseProjectile> pool = GetPool();
        if (pool == null || weapon == null) return data.fireRate;

        // Get the specific muzzle
        Transform muzzleTransform = weapon.GetMuzzle(muzzleType);
        if (muzzleTransform == null) return data.fireRate;

        Vector3 fireDirection = muzzleTransform.up;

        BaseProjectile bullet = pool.Get();
        Vector3 spawnPos = muzzleTransform.position;
        spawnPos.z = 0f;

        bullet.transform.position = spawnPos;
        bullet.transform.rotation = muzzleTransform.rotation;

        // Use local bulletType, not data.bulletType
        bullet.Initialize(bulletType, attacker, fireDirection, speedMultiplier);

        return data.fireRate;
    }
}