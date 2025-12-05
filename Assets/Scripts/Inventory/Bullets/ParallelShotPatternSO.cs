using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewParallelPattern", menuName = "ScriptableObjects/Patterns/Parallel Shot")]
public class ParallelShotPatternSO : BulletPatternSO
{

    public override void Fire(IActor source, List<MuzzleDefinition> muzzles, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool, IActor target)
    {
        if (muzzles == null) return;

        if (config.muzzleRequirements != null)
        {
            foreach (MuzzleType requiredType in config.muzzleRequirements)
            {
                FireFromMuzzleType(muzzles, requiredType, pool, config, source, direction, target);
            }
        }
    }

    private void FireFromMuzzleType(List<MuzzleDefinition> muzzles, MuzzleType typeToFind, ObjectPool<BaseProjectile> pool, BulletTypeSO config, IActor source, Vector3 direction, IActor target)
    {
        foreach (var muzzleDef in muzzles)
        {
            if (muzzleDef.type == typeToFind && muzzleDef.transform != null)
            {
                SpawnProjectile(pool, config, source, muzzleDef.transform.position, direction, target);
            }
        }
    }
}