using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A centralized factory configuration that holds references to all bullet types.
/// </summary>
[CreateAssetMenu(fileName = "BulletFactory", menuName = "Game/Bullet Factory")]
public class BulletFactory : ScriptableObject
{
    [Header("Configuration")]
    [Tooltip("List of all available bullet types in the game.")]
    [SerializeField] private BulletTypeSO[] allBulletTypes;

    private Dictionary<BulletType, BulletTypeSO> typeLookup;

    /// <summary>
    /// Initializes the lookup dictionary. 
    /// </summary>
    private void InitLookup()
    {
        if (typeLookup == null || typeLookup.Count != allBulletTypes.Length)
        {
            typeLookup = new Dictionary<BulletType, BulletTypeSO>();
            foreach (var so in allBulletTypes)
            {
                if (!typeLookup.ContainsKey(so.type))
                {
                    typeLookup.Add(so.type, so);
                }
            }
        }
    }

    public BulletTypeSO GetBulletConfig(BulletType type)
    {
        InitLookup();
        if (typeLookup.TryGetValue(type, out BulletTypeSO config))
        {
            return config;
        }

        Debug.LogError($"[BulletFactory] No configuration found for BulletType: {type}");
        return null;
    }

    public BulletTypeSO[] GetAllTypes()
    {
        return allBulletTypes;
    }
}