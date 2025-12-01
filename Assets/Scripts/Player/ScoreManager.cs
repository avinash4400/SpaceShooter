using UnityEngine;

/// <summary>
/// Manages the player's score state.
/// Listens for AddScore events and broadcasts ScoreUpdated events.
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    private int currentScore = 0;

    // Optional: High score tracking could go here

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnAddScore += AddScore;

            // Optional: Reset score on Game Start via GameplayManager state change?
            // For now, we rely on manual reset or Init.
            GameplayManager.OnGameStateChanged += HandleGameStateChange;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnAddScore -= AddScore;
            GameplayManager.OnGameStateChanged -= HandleGameStateChange;
        }
    }

    private void HandleGameStateChange(GameState newState)
    {
        // Reset score when returning to Title or starting fresh
        if (newState == GameState.PreStage || newState == GameState.TitleScreen)
        {
            ResetScore();
        }
    }

    private void AddScore(int amount)
    {
        currentScore += amount;

        // Notify UI
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerScoreUpdated(currentScore);
        }
    }

    private void ResetScore()
    {
        currentScore = 0;
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerScoreUpdated(currentScore);
        }
    }

    // Public getter if needed for Save System
    public int GetScore() => currentScore;
}