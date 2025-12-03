using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Displays the Level Name at the start of a level.
/// Listens to the global EventManager for the OnLevelStarted event.
/// Refactored to use Async/Await instead of Coroutines.
/// </summary>
public class LevelDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The TextMeshPro element to display the level name.")]
    [SerializeField] private TMP_Text levelText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1.0f;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeOutDuration = 1.0f;

    private CancellationTokenSource _cts;

    void Awake()
    {
        // Ensure text is hidden at start via Alpha
        if (levelText != null)
        {
            SetAlpha(0f);
        }
    }

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnLevelStarted += ShowLevelName;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnLevelStarted -= ShowLevelName;
        }

        // Cancel any running animation when disabled
        CancelAnimation();
    }

    void OnDestroy()
    {
        // Ensure tasks don't try to access destroyed objects
        CancelAnimation();
    }

    private void ShowLevelName(LevelSO level)
    {
        if (levelText == null || level == null) return;

        levelText.text = level.levelName;

        // Stop any running animation to restart
        CancelAnimation();
        _cts = new CancellationTokenSource();

        // Fire and forget the async method
        _ = AnimateText(_cts.Token);
    }

    private void CancelAnimation()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private async Task AnimateText(CancellationToken token)
    {
        try
        {
            // 1. Fade In
            await FadeRoutine(0f, 1f, fadeInDuration, token);

            // 2. Wait
            // Convert seconds to milliseconds for Task.Delay
            await Task.Delay((int)(displayDuration * 1000), token);

            // 3. Fade Out
            await FadeRoutine(1f, 0f, fadeOutDuration, token);
        }
        catch (System.OperationCanceledException)
        {
            // Animation was cancelled, do nothing or reset state if needed
        }
    }

    private async Task FadeRoutine(float startAlpha, float endAlpha, float duration, CancellationToken token)
    {
        float elapsed = 0f;
        SetAlpha(startAlpha);

        while (elapsed < duration)
        {
            // Throw exception if cancellation was requested
            token.ThrowIfCancellationRequested();

            // Await Task.Yield to wait for the next frame (like yield return null)
            await Task.Yield();

            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetAlpha(newAlpha);
        }

        SetAlpha(endAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (levelText != null)
        {
            Color c = levelText.color;
            c.a = alpha;
            levelText.color = c;
        }
    }
}