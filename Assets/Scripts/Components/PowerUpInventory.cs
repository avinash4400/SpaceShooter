using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the collection, selection, and activation of Power-Ups.
/// </summary>
public class PowerUpInventory : MonoBehaviour, IGameComponent
{
    public static event Action<PowerUpDataSO> OnPowerUpSelected;
    public static event Action<List<PowerUpDataSO>> OnInventoryUpdated;
    public static event Action<PowerUpDataSO> OnPowerUpActivated;

    private IActor actor;

    private List<PowerUpDataSO> collectedPowerUps = new List<PowerUpDataSO>();

    private int selectedIndex = -1;

    public void Initialize(IActor actor)
    {
        this.actor = actor;

        if (collectedPowerUps.Count > 0)
        {
            selectedIndex = 0;
            BroadcastSelection();
        }
    }

    void OnEnable()
    {
        PlayerController.OnSwitchPowerupInput += CycleSelection;
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

        if (collectedPowerUps.Count == 1)
        {
            selectedIndex = 0;
            BroadcastSelection();
        }

        OnInventoryUpdated?.Invoke(collectedPowerUps);
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
            data.effect.Apply(actor);

            if (data.effect.duration > 0)
            {
                StartCoroutine(DurationCoroutine(data.effect));
            }

            OnPowerUpActivated?.Invoke(data);
        }

        collectedPowerUps.RemoveAt(selectedIndex);

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
        if (actor != null) 
        {
            effect.Remove(actor);
        }
    }
}