using UnityEngine;

/// <summary>
/// Manages all spawning activities (Player and Enemies).
/// Listens to GameState changes to trigger appropriate spawning logic.
/// </summary>
public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Dependencies")]
    [Tooltip("Reference to the PlayerSpawner component in the scene.")]
    [SerializeField] private PlayerSpawner playerSpawner;
    // EnemySpawner reference will be added here later (Day 6)

    protected override void Awake()
    {
        base.Awake();
        // Automatically try to find the PlayerSpawner if not set in inspector
        if (playerSpawner == null)
        {
            playerSpawner = FindAnyObjectByType<PlayerSpawner>();
        }
    }

    void OnEnable()
    {
        // Subscribe to the Gameplay Manager's state change event
        GameplayManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
        // Unsubscribe
        GameplayManager.OnGameStateChanged -= OnGameStateChanged;
    }

    /// <summary>
    /// Event listener for state changes from the GameplayManager.
    /// </summary>
    /// <param name="newState">The new GameState.</param>
    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.PreStage:
                HandlePreStageSpawn();
                break;
            case GameState.StageActive:
                HandleStageActiveSpawning();
                break;
            case GameState.GameOver:
                HandleGameOverCleanup();
                break;
        }
    }

    private void HandlePreStageSpawn()
    {
        if (playerSpawner != null)
        {
            // Spawn the player instance (now type-safe as Player)
            Player playerInstance = playerSpawner.SpawnPlayer();

            // Optional: Do initial setup on the player instance here
        }
        else
        {
            Debug.LogError("[SpawnManager] PlayerSpawner is missing!");
        }
    }

    private void HandleStageActiveSpawning()
    {
        // Start enemy spawning routine here (for Day 6 implementation)
    }

    private void HandleGameOverCleanup()
    {
        // Stop all ongoing spawning routines (if any)
    }
}