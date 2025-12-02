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

    [Tooltip("Reference to the scroller script to enable/disable movement.")]
    [SerializeField] private ParallaxScroller scroller;

    [Header("Default Sprites")]
    [Tooltip("Sprite to display on the Title Screen.")]
    [SerializeField] private Sprite titleSprite;

    // Level backgrounds handle specific visuals via LevelSO.

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
                // Show Title Art, Disable Scrolling
                SetSprite(titleSprite);
                EnableScroll(false);
                break;

            case GameState.PreStage:
                // Transitioning to game. 
                // Sprite will be set by OnLevelStarted shortly.
                EnableScroll(true);
                break;

            case GameState.StageActive:
            case GameState.StageClear:
            case GameState.GameOver:
                // Ensure scroll is active during gameplay states
                EnableScroll(true);
                break;

            case GameState.Pause:
                // Scroll behavior during pause is handled by Time.timeScale usually
                break;
        }
    }

    private void HandleLevelStarted(LevelSO level)
    {
        // Apply the specific sprite for this level
        // Requires LevelSO to have 'public Sprite levelBackgroundSprite;'
        if (level.levelBackgroundSprite != null)
        {
            SetSprite(level.levelBackgroundSprite);
        }
        EnableScroll(true);
    }

    private void SetSprite(Sprite sprite)
    {
        if (backgroundRenderer != null && sprite != null)
        {
            backgroundRenderer.sprite = sprite;
        }
    }

    private void EnableScroll(bool enable)
    {
        if (scroller != null)
        {
            scroller.enabled = enable;
        }
    }
}