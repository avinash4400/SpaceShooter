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

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (sequence.Count == 0) return currentPos;

        float totalTime = 0f;
        foreach (var step in sequence) totalTime += step.duration;

        float currentCycleTime = timeAlive % totalTime;
        float timeAccumulator = 0f;

        foreach (var step in sequence)
        {
            if (currentCycleTime < timeAccumulator + step.duration)
            {
                return step.strategy.CalculateMovement(currentPos, target, currentCycleTime, speed, ref runtimeState);
            }
            timeAccumulator += step.duration;
        }

        return currentPos;
    }
}