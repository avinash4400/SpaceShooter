using UnityEngine;

/// <summary>
/// Dedicated component responsible for instantiating the Player prefab.
/// It works closely with the SpawnManager during stage setup.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The Player prefab with the Player.cs component attached.")]
    [SerializeField] private Player playerPrefab; // CHANGED FROM GameObject TO Player

    [Header("Spawn Position")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(0f, -4f, 0f);

    private Player currentPlayerInstance; // CHANGED FROM GameObject TO Player

    /// <summary>
    /// Instantiates the player prefab at the designated spawn position.
    /// </summary>
    /// <returns>The newly created Player component instance.</returns>
    public Player SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] Player Prefab reference is missing!");
            return null;
        }

        // Destroy any existing player instance before spawning a new one
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance.gameObject); // Destroy the GameObject
        }

        // Instantiate and store the Player component reference
        currentPlayerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Player spawned at {spawnPosition}.");

        return currentPlayerInstance;
    }
}