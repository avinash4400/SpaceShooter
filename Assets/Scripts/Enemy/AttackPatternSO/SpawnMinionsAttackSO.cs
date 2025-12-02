using UnityEngine;

[CreateAssetMenu(fileName = "SpawnMinionsAttack", menuName = "Game/Enemy/Attack/Spawn Minions")]
public class SpawnMinionsAttackSO : EnemyAttackSO
{
    [Header("Minion Settings")]
    [SerializeField] private Enemy minionPrefab;
    [SerializeField] private EnemyDataSO minionConfig;
    [SerializeField] private int count = 2;
    [SerializeField] private float spawnRadius = 2f;

    public override float ExecuteAttack(IActor attacker, EnemyWeapon weapon, IActor target, EnemyDataSO data)
    {
        if (LevelManager.Instance == null) return attackCooldown;

        Vector3 center = attacker.GetTransform().position;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * spawnRadius;
            Vector3 spawnPos = center + offset;
            spawnPos.z = 0f;

            Enemy minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity);
            minion.Initialize(minionConfig, target);
        }

        return attackCooldown;
    }
}