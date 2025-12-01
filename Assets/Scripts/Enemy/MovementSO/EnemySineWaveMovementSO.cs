using UnityEngine;

[CreateAssetMenu(fileName = "EnemySineWaveMovement", menuName = "Game/Enemy/Movement/Sine Wave")]
public class EnemySineWaveMovementSO : EnemyMovementSO
{
    [SerializeField] private Vector3 travelDirection = Vector3.down;
    [SerializeField] private float frequency = 2f;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private Vector3 waveAxis = Vector3.right;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref Vector3? storedPosition)
    {
        Vector3 linearMove = travelDirection.normalized * speed * Time.fixedDeltaTime;

        float currentWave = Mathf.Sin(timeAlive * frequency) * amplitude;
        float prevWave = Mathf.Sin((timeAlive - Time.fixedDeltaTime) * frequency) * amplitude;
        float waveDelta = currentWave - prevWave;

        Vector3 waveMove = waveAxis.normalized * waveDelta;

        return currentPos + linearMove + waveMove;
    }
}