using UnityEngine;

/// <summary>
/// Defines a single lootable object.
/// Holds the prefab that will be instantiated.
/// </summary>
[CreateAssetMenu(fileName = "LootItem", menuName = "Game/Loot/Loot Item")]
public class LootItemSO : ScriptableObject
{
    [Tooltip("The name used for logs/debugging.")]
    public string itemName;

    [Tooltip("The actual prefab to spawn. Must inherit from BasePickup.")]
    public BasePickup prefab;
}