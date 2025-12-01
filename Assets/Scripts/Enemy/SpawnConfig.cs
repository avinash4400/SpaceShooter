using UnityEngine;

/// <summary>
/// Container for configuration data used by SpawnPatterns.
/// Allows splitting Data (this class) from Logic (SpawnPatternSO).
/// </summary>
[System.Serializable]
public class SpawnConfig
{
    public enum SpawnMode
    {
        Continuous, // Keep spawning until condition met (e.g. infinite horde)
        FixedSquad  // Spawn 'Count' enemies once, then wait for condition (e.g. boss/squad)
    }

    [System.Serializable]
    public struct EnemyPoolEntry
    {
        public Enemy prefab;
        public EnemyDataSO config;
    }

    [Header("Mode")]
    [Tooltip("Continuous: Spawns indefinitely until quota met.\nFixedSquad: Spawns 'Count' enemies once.")]
    public SpawnMode mode = SpawnMode.Continuous;

    [Header("Single Enemy Settings")]
    public Enemy enemyPrefab;
    public EnemyDataSO enemyConfig;
    public SpawnStrategySO spawnStrategy;

    [Header("Pool Settings (For Random Patterns)")]
    [Tooltip("Used by patterns that pick random enemies (e.g. DurationPattern, EliminationPattern).")]
    public EnemyPoolEntry[] enemyPool;

    [Header("Flow Control")]
    [Tooltip("Number of enemies to spawn (Sequence) or Kills required (Elimination).")]
    public int count = 1;

    [Tooltip("Time between spawns.")]
    public float interval = 1f;

    [Tooltip("Total length of the pattern (DurationPattern).")]
    public float duration = 10f;

    [Tooltip("Delay before this specific pattern starts.")]
    public float initialDelay = 0f;
}