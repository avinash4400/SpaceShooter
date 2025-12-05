using UnityEngine;

/// <summary>
/// Component responsible for triggering loot drops.
/// </summary>
public class LootSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LootTableSO lootTable;
    [SerializeField] private SpawnStrategySO spawnStrategy;

    public void SpawnLoot()
    {
        if (lootTable == null || spawnStrategy == null)
        {
            Debug.LogWarning($"[LootSpawner] Missing Table or Strategy on {name}");
            return;
        }

        LootItemSO itemToSpawn = lootTable.GetDropItem();

        if (itemToSpawn != null && itemToSpawn.prefab != null)
        {
            Vector3 spawnPos = spawnStrategy.CalculateSpawnPosition(transform);

            BasePickup instance = Instantiate(itemToSpawn.prefab, spawnPos, Quaternion.identity);

            Debug.Log($"[LootSpawner] Spawned {itemToSpawn.itemName} at {spawnPos}");
        }
    }
}