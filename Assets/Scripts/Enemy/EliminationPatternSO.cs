using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Logic: Spawns enemies and waits until ALL are defeated.
/// </summary>
[CreateAssetMenu(fileName = "Logic_Elimination", menuName = "Game/Spawning/Patterns/Logic: Elimination")]
public class EliminationPatternSO : SpawnPatternSO
{
    public override IEnumerator Execute(LevelManager manager, SpawnConfig config)
    {
        if (config.initialDelay > 0) yield return new WaitForSeconds(config.initialDelay);

        // 1. Setup Local Tracking (State lives in the coroutine stack, not the SO)
        int targetKills = config.count;
        int currentKills = 0;

        Action<Vector3, ILootSource> onDeath = (pos, src) => { currentKills++; };

        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDeath += onDeath;
        }

        // 2. Spawn Enemies
        for (int i = 0; i < config.count; i++)
        {
            manager.SpawnEnemy(config.enemyPrefab, config.enemyConfig, config.spawnStrategy);
            yield return new WaitForSeconds(config.interval);
        }

        // 3. Wait for Kills
        while (currentKills < targetKills)
        {
            yield return null;
        }

        // 4. Cleanup
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDeath -= onDeath;
        }
    }
}