using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSinglePattern", menuName = "ScriptableObjects/Patterns/Single Shot")]
public class SingleShotPatternSO : BulletPatternSO
{
    public override void Fire(IActor source, List<MuzzleDefinition> muzzles, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool, IActor target)
    {
        // Find the Main muzzle
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

        // Fallback: If no Main defined, try to use the first available, or source transform
        if (firePoint == null)
        {
            // If we have any muzzles, use the first one
            if (muzzles != null && muzzles.Count > 0)
                firePoint = muzzles[0].transform;
            // Else fallback to actor's transform (requires casting source to Component if needed, or skipping)
        }

        if (firePoint != null)
        {
            SpawnProjectile(pool, config, source, firePoint.position, direction, target);
        }
    }
}