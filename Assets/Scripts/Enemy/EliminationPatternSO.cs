using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "Logic_Elimination", menuName = "Game/Spawning/Patterns/Logic: Elimination")]
public class EliminationPatternSO : SpawnPatternSO
{
    public override IEnumerator Execute(LevelManager manager, SpawnConfig config)
    {
        if (config.initialDelay > 0) yield return new WaitForSeconds(config.initialDelay);

        int targetKills = config.count;
        int currentKills = 0;
        int spawnedCount = 0; 

        Action<Vector3, ILootSource> onDeath = (pos, src) => { currentKills++; };

        if (EventManager.Instance != null) EventManager.Instance.OnEnemyDeath += onDeath;

        // Loop until quota reached
        while (currentKills < targetKills)
        {
            bool canSpawn = true;
            if (config.mode == SpawnConfig.SpawnMode.FixedSquad && spawnedCount >= config.count)
            {
                canSpawn = false;
            }

            if (canSpawn)
            {
                if (config.enemyPool != null && config.enemyPool.Length > 0)
                {
                    var entry = config.enemyPool[UnityEngine.Random.Range(0, config.enemyPool.Length)];
                    EnemyDataSO data = entry.config != null ? entry.config : config.enemyConfig;
                    manager.SpawnEnemy(entry.prefab, data, config.spawnStrategy);
                }
                else
                {
                    manager.SpawnEnemy(config.enemyPrefab, config.enemyConfig, config.spawnStrategy);
                }
                spawnedCount++;
            }

            // Smart Wait
            float timer = 0f;
            while (timer < config.interval)
            {
                if (currentKills >= targetKills) break;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        if (EventManager.Instance != null) EventManager.Instance.OnEnemyDeath -= onDeath;
    }
}