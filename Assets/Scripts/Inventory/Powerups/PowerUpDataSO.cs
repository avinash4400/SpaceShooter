using UnityEngine;

/// <summary>
/// Represents the data for a power-up item sitting in the inventory.
/// Separates the "Item" (Icon, Name) from the "Logic" (Effect).
/// </summary>
[CreateAssetMenu(fileName = "NewPowerUpData", menuName = "Game/PowerUp Data")]
public class PowerUpDataSO : ScriptableObject
{
    [Header("Identity")]
    public PowerUpType type;
    public string powerUpName;

    [Header("Display Info")]
    public Sprite icon;
    [TextArea] public string description;

    [Header("Behavior")]
    [Tooltip("The logic to execute when this power-up is activated.")]
    public PowerUpEffectSO effect;
}