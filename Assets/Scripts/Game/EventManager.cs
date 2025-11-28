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
    public event Action<Vector3, ILootSource> OnEnemyDeath;

    // --- Level Events ---
    public event Action<LevelSO> OnLevelStarted;
    public event Action<LevelSO> OnLevelCompleted;

    // --- Handshake Events (Player Discovery) ---
    public event Action OnPlayerRequested;
    public event Action<IActor> OnPlayerRegistered;

    /// <summary>
    /// Triggered by Player.cs
    /// </summary>
    public void TriggerPlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }

    /// <summary>
    /// Triggered by Enemy components when they die.
    /// </summary>
    public void TriggerEnemyDeath(Vector3 deathPosition, ILootSource lootSource)
    {
        OnEnemyDeath?.Invoke(deathPosition, lootSource);
    }

    /// <summary>
    /// Triggered by LevelManager when a level begins.
    /// </summary>
    public void TriggerLevelStart(LevelSO level)
    {
        OnLevelStarted?.Invoke(level);
    }

    /// <summary>
    /// Triggered by LevelManager when all waves in a level are finished.
    /// </summary>
    public void TriggerLevelCompleted(LevelSO level)
    {
        OnLevelCompleted?.Invoke(level);
    }

    /// <summary>
    /// Call this to ask the Player to identify itself.
    /// </summary>
    public void RequestPlayer()
    {
        OnPlayerRequested?.Invoke();
    }

    /// <summary>
    /// Call this to broadcast the Player's identity to listeners.
    /// </summary>
    public void RegisterPlayer(IActor player)
    {
        OnPlayerRegistered?.Invoke(player);
    }
}