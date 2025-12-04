using UnityEngine;

/// <summary>
/// Handles all audio playback in the game via EventManager subscriptions.
/// Manages Music (looping, switching) and SFX (one-shots).
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Defaults")]
    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private AudioClip uiSubmitSound;
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private AudioClip playerDeathSound; // NEW
    [SerializeField] private AudioClip defaultExplosionSound;
    [SerializeField] private AudioClip gameVictorySound;

    // State
    private LevelSO currentLevel;

    void Awake()
    {
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
    }

    void OnEnable()
    {
        GameplayManager.OnGameStateChanged += HandleStateChanged;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnLevelStarted += PlayLevelMusic;
            EventManager.Instance.OnPlayerFired += PlayFireSound;
            EventManager.Instance.OnPowerUpCollected += PlayPickupSound;
            EventManager.Instance.OnCameraShake += PlayPlayerHitSound;
            EventManager.Instance.OnExplosion += PlayExplosionSound;
            EventManager.Instance.OnGameVictory += PlayVictorySound;
            EventManager.Instance.OnUISubmit += PlayUISound;

            // NEW: Listen for Player Death
            EventManager.Instance.OnPlayerDeath += PlayPlayerDeathSound;
        }
    }

    void OnDisable()
    {
        GameplayManager.OnGameStateChanged -= HandleStateChanged;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnLevelStarted -= PlayLevelMusic;
            EventManager.Instance.OnPlayerFired -= PlayFireSound;
            EventManager.Instance.OnPowerUpCollected -= PlayPickupSound;
            EventManager.Instance.OnCameraShake -= PlayPlayerHitSound;
            EventManager.Instance.OnExplosion -= PlayExplosionSound;
            EventManager.Instance.OnGameVictory -= PlayVictorySound;
            EventManager.Instance.OnUISubmit -= PlayUISound;
            EventManager.Instance.OnPlayerDeath -= PlayPlayerDeathSound;
        }
    }

    // --- Music Handlers ---

    private void HandleStateChanged(GameState newState)
    {
        if (newState == GameState.TitleScreen)
        {
            if (titleMusic != null) PlayMusic(titleMusic);
        }
    }

    private void PlayLevelMusic(LevelSO level)
    {
        currentLevel = level;
        if (level.backgroundMusic != null)
        {
            PlayMusic(level.backgroundMusic);
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    private void PlayVictorySound()
    {
        // Stop background music to emphasize victory
        musicSource.Stop();

        if (gameVictorySound != null)
        {
            sfxSource.PlayOneShot(gameVictorySound);
        }
    }

    // --- SFX Handlers ---

    private void PlayUISound()
    {
        if (uiSubmitSound != null) sfxSource.PlayOneShot(uiSubmitSound);
    }

    private void PlayFireSound(BulletTypeSO bullet)
    {
        if (bullet.fireSound != null)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f);
            sfxSource.PlayOneShot(bullet.fireSound);
            sfxSource.pitch = 1f;
        }
    }

    private void PlayPickupSound(PowerUpDataSO data)
    {
        if (data.pickupSound != null) sfxSource.PlayOneShot(data.pickupSound);
    }

    private void PlayPlayerHitSound()
    {
        if (playerHitSound != null)
        {
            sfxSource.PlayOneShot(playerHitSound);
        }
    }

    private void PlayPlayerDeathSound()
    {
        if (playerDeathSound != null)
        {
            sfxSource.PlayOneShot(playerDeathSound);
        }
    }

    private void PlayExplosionSound(Vector3 pos, AudioClip clip)
    {
        AudioClip clipToPlay = clip != null ? clip : defaultExplosionSound;
        if (clipToPlay != null) sfxSource.PlayOneShot(clipToPlay);
    }
}