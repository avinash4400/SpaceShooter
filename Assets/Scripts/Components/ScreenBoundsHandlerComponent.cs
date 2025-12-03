using UnityEngine;

/// <summary>
/// A generic component that manages the lifecycle of an object based on screen visibility.
/// Logic: Wait for object to enter screen -> If it leaves screen -> Destroy/Disable.
/// </summary>
public class ScreenBoundsHandlerComponent : MonoBehaviour, IGameComponent
{
    // Settings
    private float boundsBuffer = 0.5f;
    private bool hasEnteredScreen = false;
    private Camera mainCamera;
    private IActor actor;

    /// <summary>
    /// IGameComponent implementation.
    /// Initializes dependencies using the actor.
    /// </summary>
    public void Initialize(IActor actor)
    {
        this.actor = actor;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) mainCamera = FindAnyObjectByType<Camera>();
        }
        hasEnteredScreen = false;
    }

    public void Configure(float buffer)
    {
        this.boundsBuffer = buffer;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) mainCamera = FindAnyObjectByType<Camera>();
        }
    }

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null || actor == null) return;

        bool isOnScreen = CheckIfOnScreen();

        if (!hasEnteredScreen)
        {
            if (isOnScreen)
            {
                hasEnteredScreen = true;
            }
        }
        else
        {
            if (!isOnScreen)
            {
                HandleExit();
            }
        }
    }

    private bool CheckIfOnScreen()
    {
        // Use the actor's transform (Rigidbody) for accurate position
        Vector3 viewPos = mainCamera.WorldToViewportPoint(actor.GetTransform().position);

        bool visibleX = viewPos.x > 0 && viewPos.x < 1;
        bool visibleY = viewPos.y > 0 && viewPos.y < 1;

        if (hasEnteredScreen)
        {
            // Logic for IS ON SCREEN (Exit Check)
            bool offScreen = viewPos.x < -boundsBuffer || viewPos.x > 1 + boundsBuffer ||
                             viewPos.y < -boundsBuffer || viewPos.y > 1 + boundsBuffer;
            return !offScreen;
        }
        else
        {
            // Logic for HAS ENTERED SCREEN (Entry Check)
            return visibleX && visibleY;
        }
    }

    private void HandleExit()
    {
        Debug.LogWarning($"[ScreenBoundsHandler] {name} exited screen and will be destroyed.");
        Destroy(gameObject);
    }
}