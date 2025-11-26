using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// The primary identity component for the Player object. 
/// It implements the IActor interface, making it the central data hub for the player.
/// It holds and initializes all core player feature components (Movement, Dash, Health) 
/// and translates local death events into global game events via the EventManager.
/// </summary>
public class Player : MonoBehaviour, IActor
{
    // References to all required components
    private HealthComponent healthComponent;
    private PlayerMovement playerMovement;
    private DashComponent dashComponent;

    // IActor State: Shared velocity data, maintained by PlayerMovement.cs
    private Vector2 currentVelocity;

    // --- IActor Implementation ---

    public Transform GetTransform() => transform;

    public Vector2 GetCurrentVelocity() => currentVelocity;

    public void SetCurrentVelocity(Vector2 velocity)
    {
        currentVelocity = velocity;
    }

    // --- Component Initialization and Event Handling ---

    void Start()
    {
        // 1. Ensure the Player object has the correct tag
        gameObject.tag = "Player";

        // 2. Aggregate and initialize all IGameComponents, ensuring they exist
        InitializeComponents();

        // 3. Subscribe to the local HealthComponent's death event.
        if (healthComponent != null)
        {
            healthComponent.OnDeath += OnLocalDeath;
        }
    }

    /// <summary>
    /// Helper method to get a component if it exists, or add it if it doesn't.
    /// </summary>
    private T GetOrAddComponent<T>() where T : Component
    {
        T component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
            Debug.LogWarning($"[Player.cs] Automatically adding missing component: {typeof(T).Name}");
        }
        return component;
    }

    /// <summary>
    /// Finds and initializes all IGameComponent implementations on this GameObject.
    /// This method now ensures critical components are present.
    /// </summary>
    private void InitializeComponents()
    {
        // 1. Ensure critical components exist (using GetOrAddComponent)
        healthComponent = GetOrAddComponent<HealthComponent>();
        playerMovement = GetOrAddComponent<PlayerMovement>();
        dashComponent = GetOrAddComponent<DashComponent>();

        // 2. Get all components implementing the IGameComponent feature interface
        // We use GetComponents here because they must exist now (step 1 guarantees it)
        IGameComponent[] gameComponents = GetComponents<IGameComponent>();

        foreach (IGameComponent component in gameComponents)
        {
            component.Initialize(this); // Inject 'this' (the IActor)
            Debug.Log($"Initialized {component.GetType().Name} with IActor reference.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= OnLocalDeath;
        }
    }

    /// <summary>
    /// Handles the local death event from the HealthComponent.
    /// This translates the low-level component event into a high-level global event.
    /// </summary>
    /// <param name="deadObject">The GameObject that died (should always be this GameObject).</param>
    private void OnLocalDeath(GameObject deadObject)
    {
        if (EventManager.Instance != null)
        {
            Debug.Log("Player identity confirmed death. Triggering global event.");
            EventManager.Instance.TriggerPlayerDeath();
        }
    }
}