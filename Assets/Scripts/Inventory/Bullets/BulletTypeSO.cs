using UnityEngine;

/// <summary>
/// Scriptable Object defining the immutable properties of a single bullet type.
/// Now includes the BulletType enum for safer identification.
/// </summary>
[CreateAssetMenu(fileName = "BulletType", menuName = "Game/Bullet Type")]
public class BulletTypeSO : ScriptableObject
{
    [Header("Identity")]
    public BulletType type; // The Enum identifier
    public string bulletName; // Display name (optional now, but good for UI)

    [Header("Behavior")]
    [Tooltip("The actual projectile prefab (must inherit from BaseProjectile).")]
    public BaseProjectile projectilePrefab;

    [Header("Stats")]
    public int damage = 1;
    public float speed = 15f;
    public float lifetime = 3f;
    public bool isLaserOrRay = false;

    [Header("Ammo")]
    public bool hasLimitedAmmo = false;

    [Header("Visuals")]
    public Sprite icon;
}