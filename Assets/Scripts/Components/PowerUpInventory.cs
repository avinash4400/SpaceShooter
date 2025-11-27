using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the collection, selection, and activation of Power-Ups.
/// Implements IGameComponent for initialization.
/// </summary>
public class PowerUpInventory : MonoBehaviour, IGameComponent
{
    // --- Events for UI ---
    public static event Action<PowerUpDataSO> OnPowerUpSelected;
    public static event Action<List<PowerUpDataSO>> OnInventoryUpdated; // For redrawing the whole list if needed
    public static event Action<PowerUpDataSO> OnPowerUpActivated;

    // --- State ---
    private IActor actor;

    // The "Infinite List" of collected power-ups
    private List<PowerUpDataSO> collectedPowerUps = new List<PowerUpDataSO>();

    private int selectedIndex = -1;

    // --- IGameComponent Implementation ---
    public void Initialize(IActor actor)
    {
        this.actor = actor;

        // Initialize state
        if (collectedPowerUps.Count > 0)
        {
            selectedIndex = 0;
            BroadcastSelection();
        }
    }

    void OnEnable()
    {
        PlayerController.OnSwitchPowerupInput += CycleSelection;
        // Subscribe to the new activation input
        PlayerController.OnActivatePowerupInput += ActivateSelectedPowerUp;
    }

    void OnDisable()
    {
        PlayerController.OnSwitchPowerupInput -= CycleSelection;
        PlayerController.OnActivatePowerupInput -= ActivateSelectedPowerUp;
    }

    /// <summary>
    /// Adds a power-up to the inventory (called by Pickup objects).
    /// </summary>
    public void AddPowerUp(PowerUpDataSO data)
    {
        if (data == null) return;

        collectedPowerUps.Add(data);

        // If this is the first item, select it automatically
        if (collectedPowerUps.Count == 1)
        {
            selectedIndex = 0;
            BroadcastSelection();
        }

        OnInventoryUpdated?.Invoke(collectedPowerUps);
        Debug.Log($"[PowerUpInventory] Collected: {data.powerUpName}");
    }

    /// <summary>
    /// Cycles through the collected power-ups.
    /// </summary>
    public void CycleSelection()
    {
        if (collectedPowerUps.Count <= 1) return;

        selectedIndex = (selectedIndex + 1) % collectedPowerUps.Count;
        BroadcastSelection();
    }

    private void BroadcastSelection()
    {
        if (selectedIndex >= 0 && selectedIndex < collectedPowerUps.Count)
        {
            OnPowerUpSelected?.Invoke(collectedPowerUps[selectedIndex]);
        }
        else
        {
            OnPowerUpSelected?.Invoke(null);
        }
    }

    /// <summary>
    /// Activates the currently selected power-up and consumes it.
    /// </summary>
    public void ActivateSelectedPowerUp()
    {
        if (selectedIndex < 0 || selectedIndex >= collectedPowerUps.Count) return;

        PowerUpDataSO data = collectedPowerUps[selectedIndex];

        if (data.effect != null)
        {
            // 1. Apply the effect strategy
            data.effect.Apply(actor);

            // 2. Handle duration (if applicable)
            if (data.effect.duration > 0)
            {
                StartCoroutine(DurationCoroutine(data.effect));
            }

            OnPowerUpActivated?.Invoke(data);
            Debug.Log($"[PowerUpInventory] Activated: {data.powerUpName}");
        }

        // 3. Consume (Remove) the item
        collectedPowerUps.RemoveAt(selectedIndex);

        // 4. Update selection logic
        if (collectedPowerUps.Count == 0)
        {
            selectedIndex = -1;
        }
        else if (selectedIndex >= collectedPowerUps.Count)
        {
            selectedIndex = collectedPowerUps.Count - 1;
        }

        BroadcastSelection();
        OnInventoryUpdated?.Invoke(collectedPowerUps);
    }

    private IEnumerator DurationCoroutine(PowerUpEffectSO effect)
    {
        yield return new WaitForSeconds(effect.duration);
        if (actor != null) // Check if player is still alive
        {
            effect.Remove(actor);
            Debug.Log($"[PowerUpInventory] Effect expired: {effect.name}");
        }
    }
}