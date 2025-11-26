/// <summary>
/// Defines the various states of the game flow.
/// </summary>
public enum GameState
{
    TitleScreen,      // Initial state, waiting for player input (Press SPACE to start)
    PreStage,         // Setup phase (e.g., loading level assets, initial score reset)
    StageActive,      // Main gameplay loop (movement, shooting, spawning)
    StageClear,       // Stage finished successfully (waves completed or timer ended)
    GameOver,         // Player HP reached 0
    Pause             // Game is temporarily suspended (optional)
}