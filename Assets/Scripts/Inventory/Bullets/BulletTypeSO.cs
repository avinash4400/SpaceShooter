using System.Collections.Generic;
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

    [Tooltip("Specific muzzle types required for this bullet's pattern (e.g. LeftWing, RightWing).")]
    public List<MuzzleType> muzzleRequirements;

    [Header("Stats")]
    public int projectileCount = 1;
    public int damage = 1;
    public float speed = 15f;
    public float lifetime = 3f;
    [Tooltip("Time in seconds between shots.")]
    public float fireRate = 0.2f;

    [Header("Ammo")]
    public bool hasLimitedAmmo = false;

    [Header("Visuals & Audio")]
    public Sprite icon;
    public AudioClip fireSound;
    public AudioClip hitSound;
}