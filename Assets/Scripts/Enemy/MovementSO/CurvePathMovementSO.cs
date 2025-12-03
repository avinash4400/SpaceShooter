using UnityEngine;

/// <summary>
/// Moves the enemy linearly across the screen (Left<->Right) while curving Y.
/// If the enemy goes off-screen, it wraps around to the starting side to loop the path.
/// </summary>
[CreateAssetMenu(fileName = "CurvePathMovement", menuName = "Game/Enemy/Movement/Curve Path")]
public class CurvePathMovementSO : EnemyMovementSO
{
    [Header("Path Settings")]
    [Tooltip("Curve applied to Y position over time.")]
    [SerializeField] private AnimationCurve yPathCurve;

    [Tooltip("Height multiplier for the curve.")]
    [SerializeField] private float curveHeightScale = 2f;

    [Tooltip("Duration of one full curve cycle (seconds).")]
    [SerializeField] private float curveDuration = 3f;

    [Header("Looping")]
    [Tooltip("Viewport padding before wrapping around (e.g. 0.2 means wrap at 1.2 or -0.2).")]
    [SerializeField] private float wrapPadding = 0.2f;

    // State container
    private class CurveState
    {
        public Vector3 initialPos;
        public float directionX; // 1 for Right, -1 for Left
        public bool initialized;
    }

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        Camera cam = Camera.main;
        if (cam == null) return currentPos;

        // 1. Initialize State
        if (runtimeState == null || !(runtimeState is CurveState))
        {
            runtimeState = new CurveState();
        }

        CurveState state = (CurveState)runtimeState;

        if (!state.initialized)
        {
            state.initialPos = currentPos;

            // Determine direction based on spawn side relative to screen center
            Vector3 viewportPos = cam.WorldToViewportPoint(currentPos);
            state.directionX = (viewportPos.x < 0.5f) ? 1f : -1f; // If left, go right.
            state.initialized = true;
        }

        // 2. Calculate Horizontal Movement (Linear)
        float moveX = state.directionX * speed * Time.fixedDeltaTime;

        // 3. Calculate Vertical Movement (Curve)
        // We calculate the Y offset based on time and apply it relative to the INITIAL Y.
        float curveT = (timeAlive % curveDuration) / curveDuration; // 0 to 1
        float yOffset = yPathCurve.Evaluate(curveT) * curveHeightScale;

        // Construct new position
        Vector3 nextPos = currentPos;
        nextPos.x += moveX;
        nextPos.y = state.initialPos.y + yOffset;
        nextPos.z = 0f;

        // 4. Handle Wrapping (Infinite Loop)
        Vector3 nextViewportPos = cam.WorldToViewportPoint(nextPos);

        // Moving Right -> Check Right Edge
        if (state.directionX > 0 && nextViewportPos.x > (1f + wrapPadding))
        {
            // Teleport to Left Edge
            Vector3 wrapScreenPos = cam.ViewportToWorldPoint(new Vector3(0f - wrapPadding, nextViewportPos.y, nextViewportPos.z));
            nextPos.x = wrapScreenPos.x;
        }
        // Moving Left -> Check Left Edge
        else if (state.directionX < 0 && nextViewportPos.x < (0f - wrapPadding))
        {
            // Teleport to Right Edge
            Vector3 wrapScreenPos = cam.ViewportToWorldPoint(new Vector3(1f + wrapPadding, nextViewportPos.y, nextViewportPos.z));
            nextPos.x = wrapScreenPos.x;
        }

        return nextPos;
    }
}