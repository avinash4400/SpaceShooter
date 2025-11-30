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

        Vector3 fireDirection = muzzle.up;

        BaseProjectile bullet = bulletPool.Get();

        // Force spawn position to Z=0
        Vector3 spawnPos = muzzle.position;
        spawnPos.z = 0f;

        bullet.transform.position = spawnPos;
        bullet.transform.rotation = muzzle.rotation;

        bullet.Initialize(data.bulletType, attacker, fireDirection, speedMultiplier);
    }
}