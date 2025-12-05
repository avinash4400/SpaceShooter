using UnityEngine;

/// <summary>
/// Manages the background visuals using a single SpriteRenderer.
/// Swaps sprites dynamically for Title and Levels.
/// </summary>
public class BackgroundController : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("The single SpriteRenderer used for all backgrounds.")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Default Sprites")]
    [Tooltip("Sprite to display on the Title Screen.")]
    [SerializeField] private Sprite titleSprite;


    void OnEnable()
    {
        GameplayManager.OnGameStateChanged += HandleStateChanged;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnLevelStarted += HandleLevelStarted;
        }
    }

    void OnDisable()
    {
        GameplayManager.OnGameStateChanged -= HandleStateChanged;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnLevelStarted -= HandleLevelStarted;
        }
    }

    private void HandleStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.TitleScreen:
                SetSprite(titleSprite);
                break;

            case GameState.PreStage:
                break;

            case GameState.StageActive:
            case GameState.StageClear:
            case GameState.GameOver:
                break;

            case GameState.Pause:
                break;
        }
    }

    private void HandleLevelStarted(LevelSO level)
    {
        if (level.levelBackgroundSprite != null)
        {
            SetSprite(level.levelBackgroundSprite);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (backgroundRenderer != null && sprite != null)
        {
            backgroundRenderer.sprite = sprite;
        }
    }
}