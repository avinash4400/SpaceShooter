using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerUpData", menuName = "Game/PowerUp Data")]
public class PowerUpDataSO : ScriptableObject
{
    [Header("Identity")]
    public PowerUpType type;
    public string powerUpName;

    [Header("Display Info")]
    public Sprite icon;
    [TextArea] public string description;
    public AudioClip pickupSound; // New

    [Header("Behavior")]
    [Tooltip("The logic to execute when this power-up is activated.")]
    public PowerUpEffectSO effect;
}