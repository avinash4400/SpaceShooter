using UnityEngine;

/// <summary>
/// A condition based on a random percentage chance.
/// </summary>
[CreateAssetMenu(fileName = "ChanceCondition", menuName = "Game/Loot/Conditions/Percentage Chance")]
public class PercentageConditionSO : LootConditionSO
{
    [Range(0f, 1f)]
    [Tooltip("Probability of success (0.0 to 1.0).")]
    [SerializeField] private float chance = 0.5f;

    public override bool CanSpawn()
    {
        return Random.value <= chance;
    }
}