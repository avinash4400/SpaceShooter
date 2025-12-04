using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// The primary identity component for the Player object. 
/// Implements the Provider side of the Handshake Pattern.
/// </summary>
public class Player : MonoBehaviour, IActor
{
    // References to all required components
    private HealthComponent healthComponent;
    private PlayerMovement playerMovement;
    private DashComponent dashComponent;
    private IGameComponent[] gameComponents;
    private Rigidbody rb;

    // IActor State
    private Vector2 currentVelocity;

    // --- IActor Implementation ---

    public Transform GetTransform() => GetRigidbody().transform;
    public Vector2 GetCurrentVelocity() => currentVelocity;
    public void SetCurrentVelocity(Vector2 velocity) => currentVelocity = velocity;
    public Rigidbody GetRigidbody() => rb;

    public T GetAttachedComponent<T>() where T : IGameComponent
    {
        return gameComponents.OfType<T>().FirstOrDefault();
    }

    // --- Lifecycle & Handshake ---

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerRequested += BroadcastSelf;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerRequested -= BroadcastSelf;
        }
    }

    void Start()
    {
        gameObject.tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");

        InitializeComponents();

        // Subscribe to local health events to forward them globally
        if (healthComponent != null)
        {
            healthComponent.OnDeath += OnLocalDeath;
            healthComponent.OnHealthChanged += OnLocalHealthChanged;

            // NEW: Listen for damage impact
            healthComponent.OnHit += OnLocalHit;

            // Initial broadcast so UI updates on start
            OnLocalHealthChanged(gameObject, healthComponent.CurrentHealth);
        }

        BroadcastSelf();
    }

    private void BroadcastSelf()
    {
        if (EventManager.Instance != null)
        {
            Debug.Log("[Player] Broadcasting identity via Handshake.");
            EventManager.Instance.RegisterPlayer(this);
        }
    }

    // --- Initialization ---

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

    private void InitializeComponents()
    {
        healthComponent = GetOrAddComponent<HealthComponent>();
        playerMovement = GetOrAddComponent<PlayerMovement>();
        dashComponent = GetOrAddComponent<DashComponent>();
        rb = GetComponentInChildren<Rigidbody>();

        gameComponents = GetComponents<IGameComponent>();

        foreach (IGameComponent component in gameComponents)
        {
            component.Initialize(this);
            Debug.Log($"Initialized {component.GetType().Name} with IActor reference.");
        }
    }

    void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= OnLocalDeath;
            healthComponent.OnHealthChanged -= OnLocalHealthChanged;
            healthComponent.OnHit -= OnLocalHit;
        }
    }

    private void OnLocalDeath(GameObject deadObject)
    {
        if (EventManager.Instance != null)
        {
            Debug.Log("Player identity confirmed death. Triggering global event.");
            EventManager.Instance.TriggerPlayerDeath();
        }
    }

    private void OnLocalHealthChanged(GameObject source, int currentHealth)
    {
        if (EventManager.Instance != null && healthComponent != null)
        {
            EventManager.Instance.TriggerPlayerHealthChanged(currentHealth, healthComponent.MaxHealth);
        }
    }

    // NEW: Forward hit event to EventManager for Screen Shake
    private void OnLocalHit(GameObject source)
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerCameraShake();
        }
    }
}