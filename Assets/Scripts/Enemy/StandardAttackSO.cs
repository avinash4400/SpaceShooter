using UnityEngine;

/// <summary>
/// Concrete Attack Strategy: Standard Forward Fire.
/// Fires a bullet from the specific muzzle point in the direction the muzzle is facing.
/// </summary>
[CreateAssetMenu(fileName = "StandardAttack", menuName = "Game/Enemy/Attack/Standard")]
public class StandardAttackSO : EnemyAttackSO
{
    public override float ExecuteAttack(
        IActor attacker,
        EnemyWeapon weapon,
        IActor target,
        EnemyDataSO data)
    {
        // Default to cooldown even if firing fails (to prevent infinite loop spam)
        if (weapon == null) return attackCooldown;

        ObjectPool<BaseProjectile> pool = GetPool();
        if (pool == null) return attackCooldown;

        Transform muzzleTransform = weapon.GetMuzzle(muzzleType);
        if (muzzleTransform == null) return attackCooldown;

        Vector3 fireDirection = muzzleTransform.up;

        BaseProjectile bullet = pool.Get();
        Vector3 spawnPos = muzzleTransform.position;
        spawnPos.z = 0f;

        bullet.transform.position = spawnPos;
        bullet.transform.rotation = muzzleTransform.rotation;

        // Pass the locally configured speedMultiplier and the target for homing
        bullet.Initialize(bulletType, attacker, fireDirection, speedMultiplier, target);

        return attackCooldown;
    }
}