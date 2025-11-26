/// <summary>
/// Defines the unique identifiers for different bullet types.
/// Using an Enum is safer and more performant than string comparisons.
/// </summary>
public enum BulletType
{
    SingleShot,
    DoubleShot,
    TripleShot,
    LaserBeam,
    // Add future types here (e.g., HomingMissile, Bomb)
}