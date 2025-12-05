/// <summary>
/// Interface for all feature components (Movement, Dash, Shooting, Health).
/// This provides a clean dependency injection method, ensuring components 
/// </summary>
public interface IGameComponent
{
    /// <summary>
    /// Initializes the component with a reference to its owning Actor.
    /// </summary>
    /// <param name="actor">The IActor interface of the owning entity.</param>
    void Initialize(IActor actor);
}