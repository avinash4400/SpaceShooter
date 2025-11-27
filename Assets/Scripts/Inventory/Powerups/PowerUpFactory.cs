using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A centralized factory/database that holds references to all available Power-Ups.
/// Essential for Loot Tables and Random Drops.
/// </summary>
[CreateAssetMenu(fileName = "PowerUpFactory", menuName = "Game/PowerUp Factory")]
public class PowerUpFactory : ScriptableObject
{
    [Header("Configuration")]
    [Tooltip("List of all available Power-Up Data assets.")]
    [SerializeField] private PowerUpDataSO[] allPowerUps;

    // Cache for O(1) lookup
    private Dictionary<PowerUpType, PowerUpDataSO> typeLookup;

    private void InitLookup()
    {
        if (typeLookup == null || typeLookup.Count != allPowerUps.Length)
        {
            typeLookup = new Dictionary<PowerUpType, PowerUpDataSO>();
            foreach (var so in allPowerUps)
            {
                if (!typeLookup.ContainsKey(so.type))
                {
                    typeLookup.Add(so.type, so);
                }
            }
        }
    }

    /// <summary>
    /// Returns a random Power-Up Data asset.
    /// Useful for loot drops.
    /// </summary>
    public PowerUpDataSO GetRandomPowerUp()
    {
        if (allPowerUps.Length == 0) return null;
        return allPowerUps[Random.Range(0, allPowerUps.Length)];
    }

    /// <summary>
    /// Retrieves specific Power-Up Data by its Enum type.
    /// </summary>
    public PowerUpDataSO GetPowerUpByType(PowerUpType type)
    {
        InitLookup();
        if (typeLookup.TryGetValue(type, out PowerUpDataSO data))
        {
            return data;
        }

        Debug.LogWarning($"[PowerUpFactory] PowerUp not found for type: {type}");
        return null;
    }

    public PowerUpDataSO[] GetAllPowerUps()
    {
        return allPowerUps;
    }
}