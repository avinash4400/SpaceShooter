using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines a segment of gameplay.
/// Holds a list of Steps, where each Step pairs a Logic Pattern with a Data Config.
/// </summary>
[CreateAssetMenu(fileName = "NewWave", menuName = "Game/Spawning/Wave")]
public class WaveSO : ScriptableObject
{
    [System.Serializable]
    public struct WaveStep
    {
        [Tooltip("The Logic Strategy (e.g. 'StandardSequence', 'EliminationLogic').")]
        public SpawnPatternSO patternLogic;

        [Tooltip("The Data (e.g. '5 Drones', 'Kill 10 Enemies').")]
        public SpawnConfig config;
    }

    [Header("Wave Sequence")]
    public List<WaveStep> spawnSteps;

    [Header("Settings")]
    public bool runInParallel = false;
    public float startDelay = 2f;
}