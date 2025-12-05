using UnityEngine;
using System.Collections.Generic;

public class EnemyWeapon : MonoBehaviour, IGameComponent
{
    [Header("Visuals")]
    [SerializeField] private List<MuzzleDefinition> muzzles;

    private EnemyAttackSO attackStrategy;
    private EnemyDataSO enemyData;
    private IActor target;
    private float nextAttackTime;

    private Dictionary<MuzzleType, Transform> muzzleLookup;
    private Transform defaultMuzzle;

    public void Initialize(IActor actor)
    {
        muzzleLookup = new Dictionary<MuzzleType, Transform>();
        foreach (var def in muzzles)
        {
            if (!muzzleLookup.ContainsKey(def.type)) muzzleLookup.Add(def.type, def.transform);
        }

        if (muzzleLookup.ContainsKey(MuzzleType.Main)) defaultMuzzle = muzzleLookup[MuzzleType.Main];
        else defaultMuzzle = actor.GetTransform();
    }

    public Transform GetMuzzle(MuzzleType type)
    {
        if (muzzleLookup != null && muzzleLookup.TryGetValue(type, out Transform t)) return t;
        return defaultMuzzle;
    }

    public void Setup(EnemyAttackSO attackStrat, EnemyDataSO data, IActor playerTarget)
    {
        attackStrategy = attackStrat;
        enemyData = data;
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

            float cooldown = attackStrategy.ExecuteAttack(attacker, this, target, enemyData);

            nextAttackTime = Time.time + cooldown;
        }
    }
}