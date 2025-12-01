using UnityEngine;
using System;

/// <summary>
/// A centralized hub for global game events.
/// Inherits from Singleton to ensure a single instance persists across scenes.
/// </summary>
public class EventManager : Singleton<EventManager>
{
    // --- Global Events ---
    public event Action OnPlayerDeath;
    public event Action<int, int> OnPlayerHealthChanged;
    public event Action<Vector3, ILootSource> OnEnemyDeath;

    // --- Score Events ---
    public event Action<int> OnAddScore;
    public event Action<int> OnScoreUpdated;

    // --- Level Events ---
    public event Action<LevelSO> OnLevelStarted;
    public event Action<LevelSO> OnLevelCompleted;
    public event Action OnGameVictory;

    // --- Boss Events ---
    public event Action<HealthComponent> OnBossSpawned; // NEW

    // --- Handshake Events ---
    public event Action OnPlayerRequested;
    public event Action<IActor> OnPlayerRegistered;

    // ... Existing Trigger Methods ...

    public void TriggerPlayerDeath() => OnPlayerDeath?.Invoke();
    public void TriggerPlayerHealthChanged(int current, int max) => OnPlayerHealthChanged?.Invoke(current, max);
    public void TriggerEnemyDeath(Vector3 deathPosition, ILootSource lootSource) => OnEnemyDeath?.Invoke(deathPosition, lootSource);
    public void TriggerLevelStart(LevelSO level) => OnLevelStarted?.Invoke(level);
    public void TriggerLevelCompleted(LevelSO level) => OnLevelCompleted?.Invoke(level);
    public void TriggerGameVictory() => OnGameVictory?.Invoke();

    // NEW Trigger
    public void TriggerBossSpawned(HealthComponent bossHealth) => OnBossSpawned?.Invoke(bossHealth);

    public void RequestPlayer() => OnPlayerRequested?.Invoke();
    public void RegisterPlayer(IActor player) => OnPlayerRegistered?.Invoke(player);
    public void TriggerAddScore(int amount) => OnAddScore?.Invoke(amount);
    public void TriggerScoreUpdated(int newTotalScore) => OnScoreUpdated?.Invoke(newTotalScore);
}