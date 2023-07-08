using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    // Stores the width and height of the level.
    public int width, height;

    // The current position of the controlled box.
    (int x, int y) currentPosition;

    // Store the starting boxes
    Box[] startBoxes; // TODO: refactor to just store movable boxes.

    // Store the grid history of the current level, which is just a grid of all the positions of the boxes
    Stack<Grid> gridHistory;
    Stack<(int x, int y)> positionHistory;
    // Stores the current state of the level as a grid.
    Grid currentGrid;
    // Store the static boxes in the grid: the boxes that cannot move. This is not stored in the history.
    Grid staticGrid;
    // Stores whether transitions are happening; cannot move while transitioning
    public bool isInTransition;
    int transitionCount = 0;

    // Stores the active box, which is being controlled currently.
    [HideInInspector]
    public Box activeBox;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public Box cursor;

    Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        // Validate width and height;
        if (width <= 0 || height <= 0) {
            Debug.LogError("Invalid width or height for level!");
        }

        // Get the initial state of the level:
        startBoxes = gameObject.GetComponentsInChildren<Box>();
        currentGrid = new(width, height, startBoxes);
        staticGrid = new(width, height, startBoxes, isStationary: true);
    
        gridHistory = new Stack<Grid>();
        gridHistory.Push(currentGrid);

        // Find the active box:
        foreach (Box box in startBoxes) {
            if (box.isActiveBox) {
                if (activeBox != null) {
                    Debug.LogError("Cannot have multiple active boxes!");
                    break;
                }
                activeBox = box;
                currentPosition = activeBox.position;
                positionHistory = new Stack<(int x, int y)>();
                positionHistory.Push(currentPosition);
            }
        }
        if (activeBox == null) {
            Debug.LogError("No active box found!");
        }

        // Find the player
        foreach (Box box in startBoxes) {
            if (box.boxType == Constants.BoxTypes.Player) {
                if (player != null) {
                    Debug.LogError("Cannot have multiple players!");
                    break;
                }
                player = new(box);
            }
        }
        if (player == null) {
            Debug.LogError("No player found");
        }

        // Find the cursor
        foreach (Box box in startBoxes) {
            if (box.boxType == Constants.BoxTypes.Cursor) {
                if (cursor != null) {
                    Debug.LogError("Cannot have multiple cursors!");
                    break;
                }
                cursor = box;
            }
        }
        if (cursor == null) {
            Debug.LogError("No cursor found");
        }

        // Find controller
        controller = FindObjectOfType<Controller>();
        if (controller == null) {
            Debug.LogError("No controller found!");
        }
        controller.SetCurrentLevel(this);
    }

    // Clear the grid history, restart the level to the start.
    public void Clear() {
        while (gridHistory.Count != 1) {
            Undo(renderGrid: false);
        }
        // Render grid:
    }

    // Undo the last move.
    public void Undo(bool renderGrid = true) {
        if (gridHistory.Count <= 1) {
            Debug.LogWarning("GridHistory cannot be empty! Cannot undo.");
        }
        gridHistory.Pop();
        positionHistory.Pop();
        currentGrid = gridHistory.Peek();
        currentPosition = positionHistory.Peek();

        // RenderGrid
        if (renderGrid) {

        }
    }

    // Snap the current state before the start of the player's next turn.
    public void PushToHistory() {
        gridHistory.Push((Grid) currentGrid.Clone());
    }

    // Moves the box that is currently controlled
    // Returns whether the box really moves
    public bool MoveControlledBox((int x, int y) direction) {
        (int x, int y) newPosition = currentPosition;
        newPosition.x += direction.x;
        newPosition.y += direction.y;

        if (IsOtherControllableBox(newPosition)) {
            // move the active position only and switch the active box.
            currentPosition = newPosition;
            SwitchActiveBox(currentPosition);
            return false;
        }
        
        // Validation
        if (IsCellBlocked(newPosition)) {
            return false;
        }

        // Move box
        currentPosition = newPosition;
        activeBox.MoveBox(direction, currentGrid);

        // Update grid
        return true;
    }

    public bool IsCellEmpty((int x, int y) coordinates) {
        return currentGrid.IsCellEmpty(coordinates) && staticGrid.IsCellEmpty(coordinates);
    }

    public bool IsCellBlocked((int x, int y) coordinates) {
        return currentGrid.IsCellBlocked(coordinates) || staticGrid.IsCellBlocked(coordinates);
    }
    public bool IsCellPushable((int x, int y) coordinates) {
        return currentGrid.IsCellPushable(coordinates) || staticGrid.IsCellPushable(coordinates);
    }

    public bool IsOtherControllableBox((int x, int y) coordinates) {
        return currentGrid.GetCellBoxType(coordinates) == (int) Constants.BoxTypes.BoxControllable;
    }

    // Switches the actively controlled box.
    public void SwitchActiveBox((int x, int y) coordinates) {
        // Find new active box:
        for (int i = 0; i < startBoxes.Length; i++) {
            if (startBoxes[i].IsAtCoordinates(coordinates) && Constants.boxTypeProps[(int) startBoxes[i].boxType].isControllable) {
                activeBox.isActiveBox = false;
                startBoxes[i].isActiveBox = true;
                activeBox = startBoxes[i];
                currentPosition = coordinates;
                cursor.MoveToPosition(coordinates);
                return;
            }
        }

        Debug.LogError("No box to switch with! Cannot switch!");
    }

    public IEnumerator MovePlayer() {
        // Wait until current transition ends:
        while (isInTransition) {
            yield return null;
        }

        (int x, int y) newPosition = player.GetNewPosition();
        
        // Switch player direction if blocked
        for (int i = 0; i < Constants.directions.Length; i++) {
            // Iteratively check if the next box in line is pushable
            while (IsCellPushable(newPosition)) {
                newPosition = player.MoveInDirection(newPosition);
            }
            if (IsCellBlocked(newPosition)) {
                // cannot move in that direction if blocked, switch direction.
                player.SwitchDirection();
                newPosition = player.GetNewPosition();
                continue;
            } 

            // push all boxes in the line if any
            while (newPosition != player.GetNewPosition() ) {
                // TODO: REFACTOR: push box. FIXME!
                // The order of the pushing has to be from the end
                for (int j = 0; j < startBoxes.Length; j++) {
                    if (startBoxes[j].IsAtCoordinates(newPosition) && Constants.boxTypeProps[(int) startBoxes[j].boxType].isPushable) {
                        startBoxes[j].MoveToPosition(newPosition, currentGrid);
                        break;
                    }
                }
                newPosition = player.MoveInDirection(newPosition, inverse: true);
            }
            // can move
            player.MovePlayer(currentGrid);
            break;
        }

        // Update history at the end of every player turn
        PushToHistory();
    }

    // Tweening helpers
    public void FinishMovement() {
        if (--transitionCount == 0) isInTransition = false;
    }

    // Tweening helpers
    public void StartMovement() {
        isInTransition = true;
        transitionCount++;
    }
}
