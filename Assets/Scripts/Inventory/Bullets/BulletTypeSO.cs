using UnityEngine;

[CreateAssetMenu(fileName = "BulletType", menuName = "Game/Bullet Type")]
public class BulletTypeSO : ScriptableObject
{
    [Header("Identity")]
    public BulletType type;
    public string bulletName;

    [Header("Strategy & Visuals")]
    [Tooltip("The logic for how this bullet is fired (e.g., Single, Spread).")]
    public BulletPatternSO patternLogic;
    [Tooltip("The actual projectile prefab.")]
    public BaseProjectile projectilePrefab;

    [Header("Stats")]
    public int projectileCount = 1;
    public int damage = 1;
    public float speed = 15f;
    public float lifetime = 3f;

    [Header("Ammo")]
    public bool hasLimitedAmmo = false;

    [Header("Visuals & Audio")]
    public Sprite icon;
    public AudioClip fireSound; // New
    public AudioClip hitSound;  // New (Explosion/Impact)
}