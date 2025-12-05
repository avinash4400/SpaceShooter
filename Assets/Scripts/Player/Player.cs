using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// The primary identity component for the Player object. 
/// </summary>
public class Player : MonoBehaviour, IActor
{
    private HealthComponent healthComponent;
    private PlayerMovement playerMovement;
    private DashComponent dashComponent;
    private HealVisuals healVisualsComponent;
    private DeathVisuals deathVisualsComponent;
    private IGameComponent[] gameComponents;
    private Rigidbody rb;

    private Vector2 currentVelocity;

    public Transform GetTransform() => GetRigidbody().transform;
    public Vector2 GetCurrentVelocity() => currentVelocity;
    public void SetCurrentVelocity(Vector2 velocity) => currentVelocity = velocity;
    public Rigidbody GetRigidbody() => rb;

    public T GetAttachedComponent<T>() where T : IGameComponent
    {
        return gameComponents.OfType<T>().FirstOrDefault();
    }

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

        if (healthComponent != null)
        {
            healthComponent.OnDeath += OnLocalDeath;
            healthComponent.OnHealthChanged += OnLocalHealthChanged;
            healthComponent.OnHeal += OnHeal;

            healthComponent.OnHit += OnLocalHit;

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
        healVisualsComponent = rb.GetComponent<HealVisuals>();
        deathVisualsComponent = rb.GetComponent<DeathVisuals>();
        if(deathVisualsComponent != null)
            deathVisualsComponent.Initialize(this);
        gameComponents = GetComponents<IGameComponent>();

        foreach (IGameComponent component in gameComponents)
        {
            component.Initialize(this);
        }
    }

    void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= OnLocalDeath;
            healthComponent.OnHealthChanged -= OnLocalHealthChanged;
            healthComponent.OnHit -= OnLocalHit;
            healthComponent.OnHeal -= OnHeal;
        }
    }

    private void OnLocalDeath(GameObject deadObject)
    {
        if (EventManager.Instance != null)
        {
            Debug.Log("Player identity confirmed death. Triggering global event.");
            EventManager.Instance.TriggerPlayerDeath();
        }
        DisableAllComponents();
        if (deathVisualsComponent != null)
        {
            deathVisualsComponent.StartDeathEffect(OnDeathViualsCompleted);
        }
    }

    private void OnDeathViualsCompleted()
    {
        Destroy(this.gameObject);
    }

    private void OnLocalHealthChanged(GameObject source, int currentHealth)
    {
        if (EventManager.Instance != null && healthComponent != null)
        {
            EventManager.Instance.TriggerPlayerHealthChanged(currentHealth, healthComponent.MaxHealth);
        }
    }

    private void OnHeal(GameObject source, int currentHealth)
    {
        if(healVisualsComponent != null)
            healVisualsComponent.PlayHealEffect();
    }
    private void OnLocalHit(GameObject source)
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerCameraShake();
        }
    }

    private void DisableAllComponents()
    {
        foreach (IGameComponent component in gameComponents)
        {
            if (component is MonoBehaviour mb)
            {
                mb.enabled = false;
            }
        }
    }
}