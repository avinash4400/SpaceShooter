using UnityEngine;

[CreateAssetMenu(fileName = "TimedPhaseMovement", menuName = "Game/Enemy/Movement/Timed Phase")]
public class TimedPhaseMovementSO : EnemyMovementSO
{
    [SerializeField] private EnemyMovementSO firstPhasePattern;
    [SerializeField] private float phaseDuration = 2.0f;
    [SerializeField] private EnemyMovementSO secondPhasePattern;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (timeAlive < phaseDuration)
        {
            return firstPhasePattern != null
                ? firstPhasePattern.CalculateMovement(currentPos, target, timeAlive, speed, ref runtimeState)
                : currentPos;
        }
        else
        {
            return secondPhasePattern != null
                ? secondPhasePattern.CalculateMovement(currentPos, target, timeAlive, speed, ref runtimeState)
                : currentPos;
        }
    }
}