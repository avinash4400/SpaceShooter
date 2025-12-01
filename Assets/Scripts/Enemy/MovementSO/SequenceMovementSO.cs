using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SequenceMovement", menuName = "Game/Enemy/Movement/Sequence")]
public class SequenceMovementSO : EnemyMovementSO
{
    [System.Serializable]
    public struct MovementStep
    {
        public EnemyMovementSO strategy;
        public float duration;
    }

    [SerializeField] private List<MovementStep> sequence;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref Vector3? storedPosition)
    {
        if (sequence.Count == 0) return currentPos;

        // Calculate total cycle time
        float totalTime = 0f;
        foreach (var step in sequence) totalTime += step.duration;

        // Where are we in the cycle?
        float currentCycleTime = timeAlive % totalTime;
        float timeAccumulator = 0f;

        foreach (var step in sequence)
        {
            if (currentCycleTime < timeAccumulator + step.duration)
            {
                // Found active step
                // We pass 'currentCycleTime' as timeAlive to keep patterns consistent within the loop
                return step.strategy.CalculateMovement(currentPos, target, currentCycleTime, speed, ref storedPosition);
            }
            timeAccumulator += step.duration;
        }

        return currentPos;
    }
}