using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A table defining multiple potential drops with weights and conditions.
/// Handles the selection logic.
/// </summary>
[CreateAssetMenu(fileName = "LootTable", menuName = "Game/Loot/Loot Table")]
public class LootTableSO : ScriptableObject
{
    [System.Serializable]
    public struct LootEntry
    {
        public LootItemSO item;
        [Min(0)] public int weight;
        public LootConditionSO[] conditions; // Optional filters
    }

    [SerializeField] private LootEntry[] entries;

    /// <summary>
    /// Attempts to select an item from the table.
    /// </summary>
    /// <returns>The selected LootItemSO, or null if nothing was selected.</returns>
    public LootItemSO GetDropItem()
    {
        // 1. Filter entries based on Conditions
        List<LootEntry> validEntries = new List<LootEntry>();
        int totalWeight = 0;

        foreach (var entry in entries)
        {
            if (entry.item == null || entry.item.prefab == null) continue;

            if (CheckConditions(entry.conditions))
            {
                validEntries.Add(entry);
                totalWeight += entry.weight;
            }
        }

        if (validEntries.Count == 0 || totalWeight == 0) return null;

        // 2. Weighted Random Selection
        int rng = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        foreach (var entry in validEntries)
        {
            currentWeightSum += entry.weight;
            if (rng < currentWeightSum)
            {
                return entry.item;
            }
        }

        return validEntries.Last().item;
    }

    private bool CheckConditions(LootConditionSO[] conditions)
    {
        if (conditions == null || conditions.Length == 0) return true;

        foreach (var condition in conditions)
        {
            if (condition != null && !condition.CanSpawn())
            {
                return false;
            }
        }
        return true;
    }
}