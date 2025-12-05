using UnityEngine;

/// <summary>
/// Centralized system responsible for spawning loot.
/// </summary>
public class LootManager : Singleton<LootManager>
{
    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDeath += HandleEnemyDeathLoot;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDeath -= HandleEnemyDeathLoot;
        }
    }

    /// <summary>
    /// Reacts to an enemy death event.
    /// </summary>
    private void HandleEnemyDeathLoot(Vector3 deathPosition, ILootSource lootSource)
    {
        if (lootSource != null)
        {
            LootTableSO table = lootSource.GetLootTable();
            if (table != null)
            {
                SpawnFromTable(table, deathPosition);
            }
        }
    }

    /// <summary>
    /// Spawns loot triggered by game logic (e.g. Timer/Level Event).
    /// </summary>
    public void SpawnGlobalLoot(LootTableSO table, SpawnStrategySO strategy)
    {
        if (table == null || strategy == null) return;

        Vector3 spawnPos = strategy.CalculateSpawnPosition(transform);

        SpawnFromTable(table, spawnPos);
    }

    /// <summary>
    /// Core logic to pick an item from the table and instantiate it.
    /// </summary>
    private void SpawnFromTable(LootTableSO table, Vector3 position)
    {
        LootItemSO itemToSpawn = table.GetDropItem();

        if (itemToSpawn != null && itemToSpawn.prefab != null)
        {
            Instantiate(itemToSpawn.prefab, position, Quaternion.identity);

            Debug.Log($"[LootManager] Spawned {itemToSpawn.itemName} at {position}");
        }
    }
}