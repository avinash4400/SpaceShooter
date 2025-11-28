using UnityEngine;

/// <summary>
/// Handles the enemy's attack logic.
/// Manages the fire timer and executes the Attack Strategy.
/// </summary>
public class EnemyWeapon : MonoBehaviour, IGameComponent
{
    // Strategies
    private EnemyAttackSO attackStrategy;
    private EnemyDataSO enemyData; // Needed for bullet type reference

    // State
    private IActor target;
    private float fireRate;
    private float nextAttackTime;

    // Pooling
    private BulletPool weaponPool;

    public void Initialize(IActor actor) { }

    public void Setup(EnemyAttackSO attackStrat, EnemyDataSO data, float rate, IActor playerTarget)
    {
        attackStrategy = attackStrat;
        enemyData = data;
        fireRate = rate;
        target = playerTarget;

        // Randomize start time slightly to prevent enemies syncing up
        nextAttackTime = Time.time + Random.Range(0.5f, 2f);

        // Setup Pool
        if (enemyData.bulletType != null && enemyData.bulletType.projectilePrefab != null)
        {
            // Create a local pool for this enemy (or shared if optimized later)
            GameObject poolRoot = new GameObject($"Pool_{name}");
            weaponPool = new BulletPool(enemyData.bulletType.projectilePrefab, 5, poolRoot.transform);
        }
    }

    void Update()
    {
        if (attackStrategy != null && Time.time >= nextAttackTime)
        {
            // The enemy itself acts as the 'Attacker' IActor
            IActor attacker = GetComponent<IActor>();

            attackStrategy.ExecuteAttack(attacker, target, enemyData, weaponPool);
            nextAttackTime = Time.time + fireRate;
        }
    }
}