/// <summary>
/// Interface for all feature components (Movement, Dash, Shooting, Health).
/// This provides a clean dependency injection method, ensuring components 
/// receive the IActor reference they need to operate.
/// </summary>
public interface IGameComponent
{
    /// <summary>
    /// Initializes the component with a reference to its owning Actor.
    /// This is called centrally by the Actor script (e.g., Player.cs) on Start.
    /// </summary>
    /// <param name="actor">The IActor interface of the owning entity.</param>
    void Initialize(IActor actor);
}