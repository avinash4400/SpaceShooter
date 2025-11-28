using UnityEngine;
using System.Collections;

/// <summary>
/// Logic: Spawns enemies randomly from a pool for a set duration.
/// </summary>
[CreateAssetMenu(fileName = "Logic_Duration", menuName = "Game/Spawning/Patterns/Logic: Duration")]
public class DurationPatternSO : SpawnPatternSO
{
    public override IEnumerator Execute(LevelManager manager, SpawnConfig config)
    {
        if (config.initialDelay > 0) yield return new WaitForSeconds(config.initialDelay);

        float timer = 0f;

        // Safety check using the new struct array
        if (config.enemyPool == null || config.enemyPool.Length == 0)
        {
            Debug.LogWarning("DurationPattern: No enemy pool defined in config.");
            yield break;
        }

        while (timer < config.duration)
        {
            // Pick random enemy from the pool
            int index = Random.Range(0, config.enemyPool.Length);
            var poolEntry = config.enemyPool[index];

            // Use the specific config if present, otherwise fallback to the main single config
            EnemyDataSO enemyData = poolEntry.config != null ? poolEntry.config : config.enemyConfig;

            manager.SpawnEnemy(poolEntry.prefab, enemyData, config.spawnStrategy);

            yield return new WaitForSeconds(config.interval);
            timer += config.interval;
        }
    }
}