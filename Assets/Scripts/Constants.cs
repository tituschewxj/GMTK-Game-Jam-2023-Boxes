using UnityEngine;
public static class Constants
{
    // Number of levels;
    public readonly static int numberOfLevels = 1;

    // The possible movement directions in the grid.
    public readonly static (int x, int y)[] directions = {
        (1, 0),
        (0, 1),
        (-1, 0),
        (0, -1),
    };

    // The movement directions.
    public readonly static (int x, int y) 
        moveRight = directions[0],
        moveUp = directions[1],
        moveLeft = directions[2],
        moveDown = directions[3];

    // Stores the transition time for each box transition.
    public readonly static float transitionTime = 0.05f;

    // Transition time for menu items
    public readonly static float levelCompleteTransitionTime = 0.5f;
    public readonly static float levelCompleteNextLevelLoadTime = 0.5f;

    // Stores the keys input.
    public readonly static KeyCode 
        up = KeyCode.W,
        down = KeyCode.S,
        left = KeyCode.A,
        right = KeyCode.D,
        undo = KeyCode.R;

    // Represents the different boxes.
    public enum BoxTypes {
        Empty, // No box in the current cell
        BoxControllable, // A box that can be controlled
        BoxUncontrollable, // A box that cannot be controlled
        Player, // The player, which cannot be controlled
        Wall, // A wall, does not move
        OpenedDoor, // A door, can be toggled close
        ClosedDoor, // A door, can be toggled open
        Goal, // The goal, if the player reaches here, the game ends
        Sublevel, // A goal like position, which leads to a sublevel
        TrapPlayer, // A trap, which ends the game if moved upon by the player
        Trap, // A trap, which ends the game if moved upon by the box or player
        Button, // A button that toggles or enables a door
        Cursor, // A special box that Indicates the active box that is moved
    }

    // Map to the various properties of the different boxes.
    public static BoxProps[] boxTypeProps = {
        BoxProps.Empty(),
        BoxProps.BoxControllable(),
        BoxProps.BoxUncontrollable(),
        BoxProps.Player(),
        BoxProps.Wall(),
        BoxProps.OpenedDoor(),
        BoxProps.ClosedDoor(),
        BoxProps.Goal(),
        BoxProps.Sublevel(),
        BoxProps.TrapPlayer(),
        BoxProps.Trap(),
        BoxProps.Button(),
        BoxProps.Cursor(),
    };

    // Map to the names of the box types.
    public static string[] boxTypeNames = {
        "Empty",
        "BoxControllable",
        "BoxUncontrollable",
        "Player",
        "Wall",
        "OpenedDoor",
        "ClosedDoor",
        "Goal",
        "Sublevel",
        "TrapPlayer",
        "Trap",
        "Button",
        "Cursor",
    };
}
