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

        // Safety check
        if (config.enemyPool == null || config.enemyPool.Length == 0)
        {
            Debug.LogWarning("DurationPattern: No enemy pool defined in config.");
            yield break;
        }

        while (timer < config.duration)
        {
            int index = Random.Range(0, config.enemyPool.Length);
            var entry = config.enemyPool[index];

            EnemyDataSO data = entry.config != null ? entry.config : config.enemyConfig;

            manager.SpawnEnemy(entry.prefab, data, config.spawnStrategy);

            yield return new WaitForSeconds(config.interval);
            timer += config.interval;
        }
    }
}