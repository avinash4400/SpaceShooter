using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSinglePattern", menuName = "ScriptableObjects/Patterns/Single Shot")]
public class SingleShotPatternSO : BulletPatternSO
{
    public override void Fire(IActor source, List<MuzzleDefinition> muzzles, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool, IActor target)
    {
        Transform firePoint = null;

        if (muzzles != null)
        {
            foreach (var muzzle in muzzles)
            {
                if (muzzle.type == MuzzleType.Main)
                {
                    firePoint = muzzle.transform;
                    break;
                }
            }
        }

        if (firePoint == null)
        {
            if (muzzles != null && muzzles.Count > 0)
                firePoint = muzzles[0].transform;
        }

        if (firePoint != null)
        {
            SpawnProjectile(pool, config, source, firePoint.position, direction, target);
        }
    }
}