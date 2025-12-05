using UnityEngine;

/// <summary>
/// Dedicated component responsible for instantiating the Player prefab.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The Player prefab with the Player.cs component attached.")]
    [SerializeField] private Player playerPrefab;

    [Header("Spawn Position")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(0f, -4f, 0f);

    private Player currentPlayerInstance;

    public Player SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] Player Prefab reference is missing!");
            return null;
        }

        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance.gameObject);
        }

        Vector3 flatSpawnPos = spawnPosition;
        flatSpawnPos.z = 0f;

        currentPlayerInstance = Instantiate(playerPrefab, flatSpawnPos, Quaternion.identity);

        return currentPlayerInstance;
    }

    public void DespawnPlayer()
    {
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance.gameObject);
            currentPlayerInstance = null;
        }
    }
}