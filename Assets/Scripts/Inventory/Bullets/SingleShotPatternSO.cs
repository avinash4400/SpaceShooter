using UnityEngine;

/// <summary>
/// A versatile pattern that can handle Single, Double, Triple (Spread) shots based on the BulletTypeSO config.
/// </summary>
[CreateAssetMenu(fileName = "UniversalPattern", menuName = "Game/Patterns/Universal Pattern")]
public class SingleShotPatternSO : BulletPatternSO
{
    [Tooltip("Spread angle in degrees. Ignored if Projectile Count is 1.")]
    [SerializeField] private float spreadAngle = 15f;

    public override void Fire(IActor source, Vector3 origin, Vector3 direction, BulletTypeSO config, ObjectPool<BaseProjectile> pool, IActor target)
    {
        int count = config.projectileCount;

        // Calculate angle step (centering the spread)
        float startAngle = 0f;
        float angleStep = 0f;

        if (count > 1)
        {
            startAngle = -spreadAngle * (count - 1) / 2f;
            angleStep = spreadAngle;
        }

        for (int i = 0; i < count; i++)
        {
            // Calculate direction rotation
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Vector3 finalDirection = rotation * direction;

            // Spawn with target
            SpawnProjectile(pool, config, source, origin, finalDirection, target);
        }
    }
}