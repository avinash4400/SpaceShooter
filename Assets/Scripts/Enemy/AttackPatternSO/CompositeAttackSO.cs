using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CompositeAttack", menuName = "Game/Enemy/Attack/Composite")]
public class CompositeAttackSO : EnemyAttackSO
{
    [Tooltip("List of attack strategies to execute together.")]
    [SerializeField] private List<EnemyAttackSO> subAttacks;

    public override float ExecuteAttack(
        IActor attacker,
        EnemyWeapon weapon,
        IActor target,
        EnemyDataSO data,
        float speedMultiplier)
    {
        foreach (var attack in subAttacks)
        {
            if (attack != null)
            {
                attack.ExecuteAttack(attacker, weapon, target, data, speedMultiplier);
            }
        }

        return data.fireRate;
    }
}