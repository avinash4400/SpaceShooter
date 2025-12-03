using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the high-level UI Panels based on the current GameState.
/// Uses a switch statement to group states into specific UI screens (Title vs Game).
/// </summary>
public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public struct StatePanel
    {
        public GameState state;
        [Tooltip("The root GameObject for this screen (e.g. TitlePanel, HUDPanel).")]
        public GameObject panel;
    }

    [Header("Configuration")]
    [Tooltip("Map states to panels. For Gameplay, map the HUD to 'StageActive'.")]
    [SerializeField] private List<StatePanel> panels;

    void OnEnable()
    {
        // Subscribe to state changes
        GameplayManager.OnGameStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        GameplayManager.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        // Determine which main panel "Key" should be active based on the state group
        GameState activeKey = GameState.TitleScreen; // Default

        switch (newState)
        {
            case GameState.TitleScreen:
                activeKey = GameState.TitleScreen;
                break;

            // Group all gameplay-related states to show the HUD/Game Panel
            case GameState.PreStage:
            case GameState.StageActive:
            case GameState.StageClear:
            case GameState.Pause:
                activeKey = GameState.StageActive;
                break;
            default: // HUD remains visible; specific Game Over UI can be overlaid or handled separately if needed
                activeKey = newState;
                break;
        }

        // Apply visibility to main panels
        foreach (var entry in panels)
        {
            if (entry.panel != null)
            {
                // Enable only the panel that matches the determined key
                bool shouldActive = entry.state == activeKey;

                if (entry.panel.activeSelf != shouldActive)
                {
                    entry.panel.SetActive(shouldActive);
                }
            }
        }
    }
}