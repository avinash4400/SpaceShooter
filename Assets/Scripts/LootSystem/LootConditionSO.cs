using UnityEngine;

/// <summary>
/// Strategy for filtering loot.
/// Determines if a loot item is valid to spawn based on game state.
/// </summary>
public abstract class LootConditionSO : ScriptableObject
{
    /// <summary>
    /// Returns true if the condition is met.
    /// </summary>
    public abstract bool CanSpawn();
}