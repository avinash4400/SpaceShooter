using UnityEngine;
using System.Collections.Generic;

public class EnemyWeapon : MonoBehaviour, IGameComponent
{
    [System.Serializable]
    public struct MuzzleDefinition
    {
        public MuzzleType type;
        public Transform transform;
    }

    [Header("Visuals")]
    [SerializeField] private List<MuzzleDefinition> muzzles;

    // Strategies
    private EnemyAttackSO attackStrategy;
    private EnemyDataSO enemyData;

    // State
    private IActor target;
    private float fireRate;
    private float nextAttackTime;

    // Internal cache
    private Dictionary<MuzzleType, Transform> muzzleLookup;
    private Transform defaultMuzzle; // Fallback

    public void Initialize(IActor actor)
    {
        muzzleLookup = new Dictionary<MuzzleType, Transform>();

        // Build lookup
        foreach (var def in muzzles)
        {
            if (!muzzleLookup.ContainsKey(def.type))
            {
                muzzleLookup.Add(def.type, def.transform);
            }
        }

        // Set default muzzle (Main or Actor Transform)
        if (muzzleLookup.ContainsKey(MuzzleType.Main))
        {
            defaultMuzzle = muzzleLookup[MuzzleType.Main];
        }
        else
        {
            defaultMuzzle = actor.GetTransform();
        }
    }

    public Transform GetMuzzle(MuzzleType type)
    {
        if (muzzleLookup != null && muzzleLookup.TryGetValue(type, out Transform t))
        {
            return t;
        }
        return defaultMuzzle;
    }

    public void Setup(EnemyAttackSO attackStrat, EnemyDataSO data, float rate, IActor playerTarget)
    {
        attackStrategy = attackStrat;
        enemyData = data;
        fireRate = rate;
        target = playerTarget;
        nextAttackTime = Time.time + Random.Range(0.5f, 2f);
    }

    public void UpdateStrategy(EnemyAttackSO newAttack)
    {
        if (newAttack != null) attackStrategy = newAttack;
    }

    void Update()
    {
        if (attackStrategy != null && Time.time >= nextAttackTime)
        {
            IActor attacker = GetComponent<IActor>();
            float speedMult = enemyData != null ? enemyData.bulletSpeedMultiplier : 1.0f;

            // Pass 'this' (the weapon component) as the muzzle provider
            float cooldown = attackStrategy.ExecuteAttack(attacker, this, target, enemyData, speedMult);

            nextAttackTime = Time.time + cooldown;
        }
    }
}