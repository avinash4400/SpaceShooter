using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Spawning/Level")]
public class LevelSO : ScriptableObject
{
    public string levelName = "Level 1";

    [Header("Sequence")]
    public List<WaveSO> waves;

    [Header("Environment")]
    public AudioClip backgroundMusic;

    [Tooltip("The scrolling sprite to apply to the background for this level.")]
    public Sprite levelBackgroundSprite;
}