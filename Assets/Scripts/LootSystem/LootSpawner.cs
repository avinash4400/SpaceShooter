using UnityEngine;

/// <summary>
/// Component responsible for triggering loot drops.
/// Uses a Strategy for positioning and a LootTable for selection.
/// </summary>
public class LootSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LootTableSO lootTable;
    [SerializeField] private SpawnStrategySO spawnStrategy;

    /// <summary>
    /// Attempts to spawn loot. Call this OnDeath or OnTimer.
    /// </summary>
    public void SpawnLoot()
    {
        if (lootTable == null || spawnStrategy == null)
        {
            Debug.LogWarning($"[LootSpawner] Missing Table or Strategy on {name}");
            return;
        }

        // 1. Get Item from Table
        LootItemSO itemToSpawn = lootTable.GetDropItem();

        if (itemToSpawn != null && itemToSpawn.prefab != null)
        {
            // 2. Calculate Position
            Vector3 spawnPos = spawnStrategy.CalculateSpawnPosition(transform);

            // 3. Instantiate
            // Note: Ideally use ObjectPooler here if Pickups are pooled!
            // For now, using Instantiate as per basic requirement.
            BasePickup instance = Instantiate(itemToSpawn.prefab, spawnPos, Quaternion.identity);

            Debug.Log($"[LootSpawner] Spawned {itemToSpawn.itemName} at {spawnPos}");
        }
    }
}