using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the history of the level: position related information
public class LevelHistory 
{
    // Store the grid history of the current level, which is just a grid of all the positions of the boxes
    readonly Stack<Grid> gridHistory;
    public Stack<(int x, int y)> positionHistory;
    
    // The current position of the controlled box.
    public (int x, int y) currentPosition;
    // Stores the current state of the level as a grid.
    public Grid currentGrid;
    // Store the static boxes in the grid: the boxes that cannot move. This is not stored in the history.
    public Grid staticGrid;

    // Initialiser for the level history
    public LevelHistory(int width, int height, ref Box[] startBoxes) {
        // Validate width and height;
        if (width <= 0 || height <= 0) {
            Debug.LogError("Invalid width or height for level!");
        }

        // Get the initial state of the level:
        currentGrid = new(width, height, ref startBoxes);
        staticGrid = new(width, height, ref startBoxes, isStationary: true);
        gridHistory = new Stack<Grid>();
        gridHistory.Push(currentGrid);
        positionHistory = new Stack<(int x, int y)>();
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
        // FIXME: doesn't detect correctly
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

    public void UpdateCurrentPosition((int x, int y) coordinates) {
        currentPosition = coordinates;
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
    public bool IsCellButton((int x, int y) coordinates) {
        return staticGrid.GetCellBoxType(coordinates) == (int) Constants.BoxTypes.Button;
    }
    public bool AtSublevelPortal() {
        return staticGrid.IsCellSublevel(currentPosition);
    }
}