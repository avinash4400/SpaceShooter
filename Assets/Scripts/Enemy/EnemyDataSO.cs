using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Stats")]
    public string enemyName;
    public int maxHealth = 10;
    public int scoreValue = 100;
    public float moveSpeed = 5f;

    [Header("Loot")]
    public LootTableSO lootTable;

    [Header("Movement Strategy")]
    public EnemyMovementSO movementPattern;

    [Header("Rotation Strategy")]
    public EnemyRotationSO rotationPattern;

    [Header("Attack Strategy")]
    public EnemyAttackSO attackPattern;

    // Note: FireRate and BulletSpeed removed. 
    // They are now configured inside the EnemyAttackSO.
}