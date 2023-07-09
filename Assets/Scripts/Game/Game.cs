// A singleton object that manages the global game state.
public static class Game
{
    // Stores which levels that are completed.
    public static bool[] completedLevels = new bool[Constants.numberOfLevels];
    public enum GameStates {
        Menu, // When not in a level
        Ongoing, // When the level is ongoing
        Won, // When the level is won
        Lost, // When the level is lost
        Transition, // When transitioning to another level
        End, // When the game ends
    }
    public static GameStates currentState = GameStates.Ongoing;
}
