using UnityEngine;
using System;

/// <summary>
/// A centralized hub for global game events.
/// Inherits from Singleton to ensure a single instance persists across scenes.
/// </summary>
public class EventManager : Singleton<EventManager>
{
    // --- Global Events ---

    // Player Death (Game Over Logic)
    public event Action OnPlayerDeath;

    // Enemy Death (Score and Loot Logic)
    // Updated: Passes snapshot data (Position) and Data Source (ILootSource) 
    // instead of the Actor reference to prevent pooling race conditions.
    public event Action<Vector3, ILootSource> OnEnemyDeath;

    /// <summary>
    /// Triggered by Player.cs
    /// </summary>
    public void TriggerPlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }

    /// <summary>
    /// Triggered by Enemy components when they die.
    /// Captures state at the moment of death.
    /// </summary>
    /// <param name="deathPosition">World position where the enemy died.</param>
    /// <param name="lootSource">The loot table provider (can be null).</param>
    public void TriggerEnemyDeath(Vector3 deathPosition, ILootSource lootSource)
    {
        OnEnemyDeath?.Invoke(deathPosition, lootSource);
    }
}