using UnityEngine;

/// <summary>
/// Moves boss to a screen position, then oscillates horizontally.
/// </summary>
[CreateAssetMenu(fileName = "BossEntryHover", menuName = "Game/Enemy/Movement/Boss/Entry Hover")]
public class BossEntryHoverSO : EnemyMovementSO
{
    [Header("Entry")]
    [Tooltip("Target Viewport Position (0,0 is bottom-left, 1,1 is top-right). e.g. (0.5, 0.8)")]
    [SerializeField] private Vector2 anchorViewportPos = new Vector2(0.5f, 0.8f);
    [SerializeField] private float entrySpeedMultiplier = 2f;

    [Header("Hover")]
    [SerializeField] private float hoverAmplitude = 3f;
    [SerializeField] private float hoverFrequency = 1f;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref Vector3? storedPosition)
    {
        // 1. Calculate Target World Position
        if (Camera.main == null) return currentPos;
        Vector3 anchorPos = Camera.main.ViewportToWorldPoint(new Vector3(anchorViewportPos.x, anchorViewportPos.y, Camera.main.nearClipPlane + 10));
        anchorPos.z = 0f;

        // 2. State Check (Have we reached the anchor?)
        // We use storedPosition.x as a flag: 0 = Entering, 1 = Hovering
        // Since we can't easily store a bool, we check distance.

        // Simpler: Just calculate distance every frame. 
        // If close enough OR we've been alive long enough, switch to hover math relative to anchor.

        float dist = Vector3.Distance(currentPos, anchorPos);

        // Entering Phase
        if (dist > 0.1f && timeAlive < 5f) // Hard timeout to force hover eventually
        {
            return Vector3.MoveTowards(currentPos, anchorPos, speed * entrySpeedMultiplier * Time.fixedDeltaTime);
        }

        // Hovering Phase
        // Move relative to the Anchor Point
        float offset = Mathf.Sin(timeAlive * hoverFrequency) * hoverAmplitude;
        return anchorPos + (Vector3.right * offset);
    }
}