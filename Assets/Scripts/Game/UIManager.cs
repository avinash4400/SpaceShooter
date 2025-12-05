using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the high-level UI Panels based on the current GameState.
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
        GameplayManager.OnGameStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        GameplayManager.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        GameState activeKey = newState; 

        switch (newState)
        {
            case GameState.TitleScreen:
                activeKey = GameState.TitleScreen;
                break;

            case GameState.PreStage:
            case GameState.StageActive:
            case GameState.StageClear:
            case GameState.Pause:
                activeKey = GameState.StageActive;
                break;

            case GameState.GameVictory:
                activeKey = GameState.GameVictory;
                break;
        }

        foreach (var entry in panels)
        {
            if (entry.panel != null)
            {
                bool shouldActive = entry.state == activeKey;

                if (entry.panel.activeSelf != shouldActive)
                {
                    entry.panel.SetActive(shouldActive);
                }
            }
        }
    }
}