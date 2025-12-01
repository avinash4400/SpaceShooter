using UnityEngine;
using TMPro;

/// <summary>
/// Displays the current score.
/// Listens to the global OnScoreUpdated event.
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Settings")]
    [SerializeField] private string prefix = "SCORE: ";
    [SerializeField] private string format = "D8"; // 00000000 format

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnScoreUpdated += UpdateScoreText;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnScoreUpdated -= UpdateScoreText;
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{prefix}{newScore.ToString(format)}";
        }
    }
}