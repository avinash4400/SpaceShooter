using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Displays the Level Name at the start of a level.
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

        CancelAnimation();
    }

    void OnDestroy()
    {
        CancelAnimation();
    }

    private void ShowLevelName(LevelSO level)
    {
        if (levelText == null || level == null) return;

        levelText.text = level.levelName;

        CancelAnimation();
        _cts = new CancellationTokenSource();

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
            await FadeRoutine(0f, 1f, fadeInDuration, token);

            await Task.Delay((int)(displayDuration * 1000), token);

            await FadeRoutine(1f, 0f, fadeOutDuration, token);
        }
        catch (System.OperationCanceledException)
        {
        }
    }

    private async Task FadeRoutine(float startAlpha, float endAlpha, float duration, CancellationToken token)
    {
        float elapsed = 0f;
        SetAlpha(startAlpha);

        while (elapsed < duration)
        {
            token.ThrowIfCancellationRequested();

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