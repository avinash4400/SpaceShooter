using UnityEngine;

[CreateAssetMenu(fileName = "SelectorAttack", menuName = "Game/Enemy/Attack/AI Selector")]
public class SelectorAttackSO : EnemyAttackSO
{
    [Header("Conditions")]
    [SerializeField] private float closeRangeThreshold = 4f;

    [Header("Strategies")]
    [SerializeField] private EnemyAttackSO closeRangeAttack;
    [SerializeField] private EnemyAttackSO longRangeAttack;

    public override float ExecuteAttack(IActor attacker, EnemyWeapon weapon, IActor target, EnemyDataSO data)
    {
        if (target == null) return attackCooldown;

        float dist = Vector3.Distance(attacker.GetTransform().position, target.GetTransform().position);

        if (dist < closeRangeThreshold && closeRangeAttack != null)
        {
            return closeRangeAttack.ExecuteAttack(attacker, weapon, target, data);
        }
        else if (longRangeAttack != null)
        {
            return longRangeAttack.ExecuteAttack(attacker, weapon, target, data);
        }

        return attackCooldown;
    }
}