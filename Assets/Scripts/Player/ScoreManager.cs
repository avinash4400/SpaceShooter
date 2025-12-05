using UnityEngine;

/// <summary>
/// Manages the player's score state.
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    private int currentScore = 0;


    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnAddScore += AddScore;

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
        if (newState == GameState.PreStage || newState == GameState.TitleScreen)
        {
            ResetScore();
        }
    }

    private void AddScore(int amount)
    {
        currentScore += amount;

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
    public int GetScore() => currentScore;
}