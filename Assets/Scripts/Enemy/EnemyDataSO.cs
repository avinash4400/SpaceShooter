using UnityEngine;

/// <summary>
/// Configuration asset defining an enemy's stats and behavior strategies.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Stats")]
    public string enemyName;
    public int maxHealth = 10;
    public int scoreValue = 100;
    public float moveSpeed = 5f;

    [Header("Loot")]
    public LootTableSO lootTable; // For ILootSource implementation

    [Header("Movement Strategy")]
    public EnemyMovementSO movementPattern;

    [Header("Rotation Strategy")]
    public EnemyRotationSO rotationPattern;

    [Header("Attack Strategy")]
    public EnemyAttackSO attackPattern;
    public BulletTypeSO bulletType; // What bullet they fire
    public float fireRate = 1.5f;

    [Tooltip("Multiplier for the speed of bullets fired by this enemy. 1.0 = Base Speed.")]
    public float bulletSpeedMultiplier = 1.0f;
}