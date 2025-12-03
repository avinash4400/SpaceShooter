using UnityEngine;

/// <summary>
/// Moves the enemy from its spawn point to a calculated destination on the opposite side of the screen.
/// Applies an Animation Curve to offset the path (creating arcs, loops, or waves).
/// Loops the movement infinitely based on duration.
/// Updated to support spawning from ANY side (Top, Bottom, Left, Right).
/// </summary>
[CreateAssetMenu(fileName = "PointToPointCurve", menuName = "Game/Enemy/Movement/Point To Point Curve")]
public class PointToPointCurveMovementSO : EnemyMovementSO
{
    [Header("Path Configuration")]
    [Tooltip("The shape of the path deviation. 0=Start, 1=End. \nExample: A Bell curve makes an arc. A Sine curve makes an S-shape.")]
    [SerializeField] private AnimationCurve pathCurve = AnimationCurve.Constant(0, 1, 0);

    [Tooltip("How far the enemy deviates from the straight line path.")]
    [SerializeField] private float curveHeight = 3f;

    [Tooltip("Time (seconds) to travel from Start to End before looping.")]
    [SerializeField] private float duration = 4f;

    [Header("Destination Randomness")]
    [Tooltip("Random offset for the destination point along the target edge.")]
    [SerializeField] private float endPointVariance = 2f;

    // --- State Definition ---
    private class PathState
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public bool initialized;
    }

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        // 1. Initialize State
        if (runtimeState == null || !(runtimeState is PathState))
        {
            runtimeState = new PathState();
        }

        PathState state = (PathState)runtimeState;

        // 2. Setup Start and End points on the first frame
        if (!state.initialized)
        {
            if (Camera.main != null)
            {
                state.startPoint = currentPos;

                // Determine spawn side based on Viewport Coordinates
                // 0,0 is Bottom-Left. 1,1 is Top-Right.
                Vector3 vp = Camera.main.WorldToViewportPoint(currentPos);

                // Calculate distance to each edge
                float distLeft = vp.x;
                float distRight = 1f - vp.x;
                float distBottom = vp.y;
                float distTop = 1f - vp.y;

                // Find the smallest distance to determine which edge we are closest to
                float minDist = Mathf.Min(distLeft, distRight, distBottom, distTop);

                Vector3 targetViewport;

                // Set destination to the OPPOSITE side
                if (minDist == distTop)
                {
                    // Spawned Top -> Go Bottom (-0.1 Y), Randomize X
                    float randX = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(randX, -0.1f, vp.z);
                }
                else if (minDist == distBottom)
                {
                    // Spawned Bottom -> Go Top (1.1 Y), Randomize X
                    float randX = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(randX, 1.1f, vp.z);
                }
                else if (minDist == distLeft)
                {
                    // Spawned Left -> Go Right (1.1 X), Randomize Y
                    float randY = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(1.1f, randY, vp.z);
                }
                else // Right
                {
                    // Spawned Right -> Go Left (-0.1 X), Randomize Y
                    float randY = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(-0.1f, randY, vp.z);
                }

                // Convert back to World Space
                Vector3 destWorld = Camera.main.ViewportToWorldPoint(targetViewport);

                // Add optional extra world-space variance if desired (though viewport random handles most)
                // destWorld.x += Random.Range(-endPointVariance, endPointVariance); 

                destWorld.z = 0f;
                state.endPoint = destWorld;
                state.initialized = true;
            }
            else
            {
                return currentPos; // Safety
            }
        }

        // 3. Calculate Progress (Looping)
        // t goes from 0.0 to 1.0, then instantly resets to 0.0
        float t = (timeAlive % duration) / duration;

        // 4. Linear Path (The straight line)
        Vector3 linearPos = Vector3.Lerp(state.startPoint, state.endPoint, t);

        // 5. Curve Deviation
        // Calculate the direction vector
        Vector3 pathDirection = (state.endPoint - state.startPoint).normalized;

        // Calculate a perpendicular normal vector 
        // For 2D: (-y, x) gives a vector 90 degrees to the left
        Vector3 pathNormal = new Vector3(-pathDirection.y, pathDirection.x, 0f);

        // Apply curve
        float curveValue = pathCurve.Evaluate(t);
        Vector3 deviation = pathNormal * curveValue * curveHeight;

        Vector3 finalPos = linearPos + deviation;
        finalPos.z = 0f;

        return finalPos;
    }
}