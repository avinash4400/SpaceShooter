/// <summary>
/// Defines the various states of the game flow.
/// </summary>
public enum GameState
{
    TitleScreen,      // Initial state, waiting for player input
    PreStage,         // Setup phase
    StageActive,      // Main gameplay loop
    StageClear,       // Wave/Level finished successfully
    GameOver,         // Player HP reached 0
    Pause,            // Game suspended
    GameVictory       // All levels completed (Campaign Finished)
}