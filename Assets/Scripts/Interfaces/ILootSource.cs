/// <summary>
/// Interface for any entity that provides a loot table (e.g., Enemies, Crates).
/// Used by LootManager to retrieve drop data upon death.
/// </summary>
public interface ILootSource
{
    /// <summary>
    /// Returns the Loot Table assigned to this entity.
    /// </summary>
    LootTableSO GetLootTable();
}