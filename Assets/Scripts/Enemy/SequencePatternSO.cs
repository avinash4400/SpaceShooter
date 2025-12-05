using UnityEngine;
using System.Collections;

/// <summary>
/// Logic: Spawns X enemies separated by Y seconds.
/// </summary>
[CreateAssetMenu(fileName = "Logic_Sequence", menuName = "Game/Spawning/Patterns/Logic: Sequence")]
public class SequencePatternSO : SpawnPatternSO
{
    public override IEnumerator Execute(LevelManager manager, SpawnConfig config)
    {
        if (config.initialDelay > 0) yield return new WaitForSeconds(config.initialDelay);

        for (int i = 0; i < config.count; i++)
        {

            bool usePool = config.enemyPool != null
                  && config.enemyPool.Length > 0
                  && UnityEngine.Random.value < 0.5f;

            if (usePool)
            {
                var entry = config.enemyPool[UnityEngine.Random.Range(0, config.enemyPool.Length)];
                EnemyDataSO data = entry.config != null ? entry.config : config.enemyConfig;
                manager.SpawnEnemy(entry.prefab, data, config.spawnStrategy);
            }
            else
            {
                manager.SpawnEnemy(config.enemyPrefab, config.enemyConfig, config.spawnStrategy);
            }

            if (i < config.count - 1)
            {
                yield return new WaitForSeconds(config.interval);
            }
        }
    }
}