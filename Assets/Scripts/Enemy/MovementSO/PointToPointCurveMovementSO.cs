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


    private class PathState
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public bool initialized;
    }

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (runtimeState == null || !(runtimeState is PathState))
        {
            runtimeState = new PathState();
        }

        PathState state = (PathState)runtimeState;

        if (!state.initialized)
        {
            if (Camera.main != null)
            {
                state.startPoint = currentPos;

                Vector3 vp = Camera.main.WorldToViewportPoint(currentPos);

                float distLeft = vp.x;
                float distRight = 1f - vp.x;
                float distBottom = vp.y;
                float distTop = 1f - vp.y;

                float minDist = Mathf.Min(distLeft, distRight, distBottom, distTop);

                Vector3 targetViewport;

                if (minDist == distTop)
                {
                    float randX = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(randX, -0.1f, vp.z);
                }
                else if (minDist == distBottom)
                {
                    float randX = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(randX, 1.1f, vp.z);
                }
                else if (minDist == distLeft)
                {
                    float randY = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(1.1f, randY, vp.z);
                }
                else 
                {
                    float randY = Random.Range(0.1f, 0.9f);
                    targetViewport = new Vector3(-0.1f, randY, vp.z);
                }

                Vector3 destWorld = Camera.main.ViewportToWorldPoint(targetViewport);


                destWorld.z = 0f;
                state.endPoint = destWorld;
                state.initialized = true;
            }
            else
            {
                return currentPos; 
            }
        }

        float t = (timeAlive % duration) / duration;

        Vector3 linearPos = Vector3.Lerp(state.startPoint, state.endPoint, t);

        Vector3 pathDirection = (state.endPoint - state.startPoint).normalized;

        Vector3 pathNormal = new Vector3(-pathDirection.y, pathDirection.x, 0f);

        float curveValue = pathCurve.Evaluate(t);
        Vector3 deviation = pathNormal * curveValue * curveHeight;

        Vector3 finalPos = linearPos + deviation;
        finalPos.z = 0f;

        return finalPos;
    }
}