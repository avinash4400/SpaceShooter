using UnityEngine;

/// <summary>
/// Concrete Attack Strategy: Standard Forward Fire.
/// Fires a bullet from the specific muzzle point in the direction the muzzle is facing.
/// </summary>
[CreateAssetMenu(fileName = "StandardAttack", menuName = "Game/Enemy/Attack/Standard")]
public class StandardAttackSO : EnemyAttackSO
{
    public override void ExecuteAttack(
        IActor attacker,
        Transform muzzle,
        IActor target,
        EnemyDataSO data,
        ObjectPool<BaseProjectile> bulletPool,
        float speedMultiplier)
    {
        if (bulletPool == null || muzzle == null) return;

        // 1. Determine Direction (Muzzle Forward)
        Vector3 fireDirection = muzzle.up;

        // 2. Spawn from Pool
        BaseProjectile bullet = bulletPool.Get();

        bullet.transform.position = muzzle.position;
        bullet.transform.rotation = muzzle.rotation;

        // 3. Initialize with Speed Multiplier
        bullet.Initialize(data.bulletType, attacker, fireDirection, speedMultiplier);
    }
}