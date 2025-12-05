using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{
    // --- Global Events ---
    public event Action OnPlayerDeath;
    public event Action<int, int> OnPlayerHealthChanged;
    public event Action<Vector3, ILootSource> OnEnemyDeath;

    // --- Audio / VFX Events ---
    public event Action OnCameraShake;
    public event Action<Vector3, AudioClip> OnExplosion;
    public event Action<BulletTypeSO> OnPlayerFired;
    public event Action<PowerUpDataSO> OnPowerUpCollected;
    public event Action<AudioClip> OnPickupSound;

    // --- UI Interaction Event ---
    public event Action OnUISubmit;

    // --- Enemy Lifecycle Events ---
    public event Action<Enemy> OnEnemySpawned;
    public event Action<Enemy> OnEnemyDespawned;

    // --- Score Events ---
    public event Action<int> OnAddScore;
    public event Action<int> OnScoreUpdated;

    // --- Level Events ---
    public event Action<LevelSO> OnLevelStarted;
    public event Action<LevelSO> OnLevelCompleted;
    public event Action OnGameVictory;

    // --- Boss Events ---
    public event Action<HealthComponent> OnBossSpawned;

    // --- Handshake Events ---
    public event Action OnPlayerRequested;
    public event Action<IActor> OnPlayerRegistered;

    // ... Triggers ...

    public void TriggerPlayerDeath() => OnPlayerDeath?.Invoke();
    public void TriggerPlayerHealthChanged(int current, int max) => OnPlayerHealthChanged?.Invoke(current, max);

    public void TriggerCameraShake() => OnCameraShake?.Invoke();
    public void TriggerExplosion(Vector3 position, AudioClip clip) => OnExplosion?.Invoke(position, clip);
    public void TriggerPlayerFired(BulletTypeSO bullet) => OnPlayerFired?.Invoke(bullet);
    public void TriggerPowerUpCollected(PowerUpDataSO data) => OnPowerUpCollected?.Invoke(data);
    public void TriggerPickupSound(AudioClip clip) => OnPickupSound?.Invoke(clip);

    public void TriggerUISubmit() => OnUISubmit?.Invoke();

    public void TriggerEnemyDeath(Vector3 deathPosition, ILootSource lootSource) => OnEnemyDeath?.Invoke(deathPosition, lootSource);
    public void TriggerEnemySpawned(Enemy enemy) => OnEnemySpawned?.Invoke(enemy);
    public void TriggerEnemyDespawned(Enemy enemy) => OnEnemyDespawned?.Invoke(enemy);

    public void TriggerLevelStart(LevelSO level) => OnLevelStarted?.Invoke(level);
    public void TriggerLevelCompleted(LevelSO level) => OnLevelCompleted?.Invoke(level);
    public void TriggerGameVictory() => OnGameVictory?.Invoke();
    public void TriggerBossSpawned(HealthComponent bossHealth) => OnBossSpawned?.Invoke(bossHealth);

    public void RequestPlayer() => OnPlayerRequested?.Invoke();
    public void RegisterPlayer(IActor player) => OnPlayerRegistered?.Invoke(player);
    public void TriggerAddScore(int amount) => OnAddScore?.Invoke(amount);
    public void TriggerScoreUpdated(int newTotalScore) => OnScoreUpdated?.Invoke(newTotalScore);
}