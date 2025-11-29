using UnityEngine;

/// <summary>
/// Handles the enemy's attack logic.
/// Manages the fire timer and executes the Attack Strategy.
/// </summary>
public class EnemyWeapon : MonoBehaviour, IGameComponent
{
    [Header("Visuals")]
    [Tooltip("The Transform where bullets spawn. Create a child object on the Enemy Prefab and assign it here.")]
    [SerializeField] private Transform muzzlePoint;

    // Strategies
    private EnemyAttackSO attackStrategy;
    private EnemyDataSO enemyData;

    // State
    private IActor target;
    private float fireRate;
    private float nextAttackTime;

    // Pooling (Injected)
    private BulletPool weaponPool;

    public void Initialize(IActor actor)
    {
        if (muzzlePoint == null)
        {
            muzzlePoint = actor.GetTransform();
        }
    }

    /// <summary>
    /// Sets up the weapon with strategies and the specific bullet pool.
    /// </summary>
    public void Setup(EnemyAttackSO attackStrat, EnemyDataSO data, float rate, IActor playerTarget, BulletPool sharedPool)
    {
        attackStrategy = attackStrat;
        enemyData = data;
        fireRate = rate;
        target = playerTarget;

        // Injected dependency
        weaponPool = sharedPool;

        nextAttackTime = Time.time + Random.Range(0.5f, 2f);
    }

    void Update()
    {
        if (attackStrategy != null && Time.time >= nextAttackTime)
        {
            IActor attacker = GetComponent<IActor>();

            // Pass the bullet speed multiplier from the enemy config
            float speedMult = enemyData != null ? enemyData.bulletSpeedMultiplier : 1.0f;

            // Execute attack using the injected pool
            attackStrategy.ExecuteAttack(attacker, muzzlePoint, target, enemyData, weaponPool, speedMult);

            nextAttackTime = Time.time + fireRate;
        }
    }
}