using UnityEngine;
using System;

/// <summary>
/// A centralized hub for global game events.
/// Inherits from Singleton to ensure a single instance persists across scenes.
/// </summary>
public class EventManager : Singleton<EventManager>
{
    // Instance property is inherited from Singleton<T>
    // public static EventManager Instance { get; private set; } <-- REMOVED

    // --- Global Events ---

    // Used by GameplayManager to transition to the Game Over state.
    public event Action OnPlayerDeath;

    // Other global events would go here:
    // public event Action<int> OnScoreUpdated;
    // public event Action OnWaveCleared;

    // Awake is now handled by the base class:
    // protected override void Awake() { base.Awake(); }

    /// <summary>
    /// Wrapper method to safely invoke the OnPlayerDeath event.
    /// Called by the Player.cs component when its local HealthComponent dies.
    /// </summary>
    public void TriggerPlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
}