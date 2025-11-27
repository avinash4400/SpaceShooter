using UnityEngine;

/// <summary>
/// Configuration for a specific ammo type.
/// Links the Visuals (Prefab) with the Logic (Pattern) and the Stats (Damage/Count).
/// </summary>
[CreateAssetMenu(fileName = "BulletType", menuName = "Game/Bullet Type")]
public class BulletTypeSO : ScriptableObject
{
    [Header("Identity")]
    public BulletType type;
    public string bulletName;

    [Header("Strategy & Visuals")]
    [Tooltip("The logic for how this bullet is fired (e.g., Single, Spread).")]
    public BulletPatternSO patternLogic; // <--- The Strategy
    [Tooltip("The actual projectile prefab.")]
    public BaseProjectile projectilePrefab;

    [Header("Stats")]
    public int projectileCount = 1; // Used by the pattern
    public int damage = 1;
    public float speed = 15f;
    public float lifetime = 3f;

    [Header("Ammo")]
    public bool hasLimitedAmmo = false;

    [Header("Visuals")]
    public Sprite icon;
}