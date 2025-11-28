using UnityEngine;

/// <summary>
/// Moves the enemy in a direction while oscillating on a perpendicular axis.
/// Renamed to avoid conflict with Loot system movement.
/// </summary>
[CreateAssetMenu(fileName = "EnemySineWaveMovement", menuName = "Game/Enemy/Movement/Sine Wave")]
public class EnemySineWaveMovementSO : EnemyMovementSO
{
    [Header("General Direction")]
    [Tooltip("Primary direction of travel.")]
    [SerializeField] private Vector3 travelDirection = Vector3.down;

    [Header("Wave Settings")]
    [Tooltip("Frequency of the wave (Speed of oscillation).")]
    [SerializeField] private float frequency = 2f;

    [Tooltip("Amplitude of the wave (Width of oscillation).")]
    [SerializeField] private float amplitude = 1f;

    [Tooltip("Axis of oscillation (perpendicular to travel direction).")]
    [SerializeField] private Vector3 waveAxis = Vector3.right;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed)
    {
        // 1. Calculate linear displacement
        // We use timeAlive to calculate the wave, but for the linear part we ideally want frame-based movement 
        // to interact with physics. However, calculating absolute position based on time is smoother for waves.

        // Approach: Calculate the offset from the START position would be ideal, but Enemy doesn't pass StartPos.
        // Alternative: Calculate the delta based on the wave derivative? 
        // Simpler Alternative for this architecture: 
        // Move linearly, then add the wave offset *relative* to the linear path.
        // BUT, since we return absolute position, we need to be careful not to "teleport" if we don't have start pos.

        // Let's assume for the wave pattern to work purely on 'currentPos', we add the linear step
        // and add the *change* in sine wave value. 

        // Actually, the standard way with this signature (CurrentPos) is to apply the linear move,
        // and adding a velocity vector that represents the wave.

        Vector3 linearMove = travelDirection.normalized * speed * Time.fixedDeltaTime;

        // Wave offset calculation: Sin(t)
        float currentWave = Mathf.Sin(timeAlive * frequency) * amplitude;
        float prevWave = Mathf.Sin((timeAlive - Time.fixedDeltaTime) * frequency) * amplitude;
        float waveDelta = currentWave - prevWave;

        Vector3 waveMove = waveAxis.normalized * waveDelta;

        return currentPos + linearMove + waveMove;
    }
}