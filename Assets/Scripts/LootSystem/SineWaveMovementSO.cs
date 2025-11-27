using UnityEngine;

/// <summary>
/// Moves the object downwards while oscillating horizontally (Sine Wave).
/// </summary>
[CreateAssetMenu(fileName = "SineWaveMovement", menuName = "Game/Loot/Movement/Sine Wave")]
public class SineWaveMovementSO : LootMovementSO
{
    [Header("General Direction")]
    [Tooltip("Primary direction of travel (e.g., Down).")]
    [SerializeField] private Vector3 fallDirection = Vector3.down;

    [Header("Wave Settings")]
    [Tooltip("Frequency of the wave (Speed of oscillation).")]
    [SerializeField] private float frequency = 2f;

    [Tooltip("Amplitude of the wave (Width of oscillation).")]
    [SerializeField] private float amplitude = 1f;

    [Tooltip("Axis of oscillation (perpendicular to fall direction).")]
    [SerializeField] private Vector3 waveAxis = Vector3.right;

    public override Vector3 CalculatePosition(Vector3 startPos, float time, float speed)
    {
        // 1. Calculate linear fall position
        Vector3 linearPos = startPos + (fallDirection.normalized * speed * time);

        // 2. Calculate sine wave offset
        float waveOffset = Mathf.Sin(time * frequency) * amplitude;
        Vector3 wavePos = waveAxis.normalized * waveOffset;

        // 3. Combine
        return linearPos + wavePos;
    }
}