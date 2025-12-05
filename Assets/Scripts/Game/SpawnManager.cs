using UnityEngine;

/// <summary>
/// Manages all spawning activities (Player and Enemies).
/// </summary>
public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Dependencies")]
    [Tooltip("Reference to the PlayerSpawner component in the scene.")]
    [SerializeField] private PlayerSpawner playerSpawner;

    protected override void Awake()
    {
        base.Awake();
        if (playerSpawner == null)
        {
            playerSpawner = FindAnyObjectByType<PlayerSpawner>();
        }
    }

    void OnEnable()
    {
        GameplayManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
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
            Player playerInstance = playerSpawner.SpawnPlayer();
        }
        else
        {
            Debug.LogError("[SpawnManager] PlayerSpawner is missing!");
        }
    }

    private void HandleStageActiveSpawning()
    {
    }

    private void HandleGameOverCleanup()
    {
        if (playerSpawner != null)
        {
            playerSpawner.DespawnPlayer();
        }
    }
}