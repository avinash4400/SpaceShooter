using UnityEngine;

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

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (Camera.main == null) return currentPos;
        Vector3 anchorPos = Camera.main.ViewportToWorldPoint(new Vector3(anchorViewportPos.x, anchorViewportPos.y, Camera.main.nearClipPlane + 10));
        anchorPos.z = 0f;

        // Use bool flag: true = has arrived
        bool hasArrived = runtimeState is bool arrived && arrived;

        // 1. Calculate the ideal Hover Position based on time
        float sineOffset = Mathf.Sin(timeAlive * hoverFrequency) * hoverAmplitude;
        Vector3 targetHoverPos = anchorPos + (Vector3.right * sineOffset);

        // 2. Determine immediate target
        // If arrived, we target the calculated hover position.
        // If NOT arrived, we target the anchor first.
        Vector3 immediateTarget = hasArrived ? targetHoverPos : anchorPos;

        // 3. Move smoothly towards immediate target
        // This prevents snapping. Even if we switch states, we MoveTowards the new target point.
        float moveStep = speed * entrySpeedMultiplier * Time.fixedDeltaTime;
        Vector3 newPos = Vector3.MoveTowards(currentPos, immediateTarget, moveStep);

        // 4. Check Arrival (State Transition)
        if (!hasArrived)
        {
            float dist = Vector3.Distance(currentPos, anchorPos);

            // Check if close enough to anchor to start hovering
            if (dist < 0.1f || timeAlive > 5f)
            {
                runtimeState = true; // Set state
            }
        }

        return newPos;
    }
}