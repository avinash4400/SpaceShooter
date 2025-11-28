using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines a full game level (e.g. "Level 1", "Boss Stage").
/// Contains a sequence of Waves to execute.
/// </summary>
[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Spawning/Level")]
public class LevelSO : ScriptableObject
{
    public string levelName = "Level 1";

    [Header("Sequence")]
    public List<WaveSO> waves;

    [Header("Environment")]
    public AudioClip backgroundMusic;
    // public Sprite backgroundArt;
}