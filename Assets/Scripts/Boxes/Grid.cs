using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

// Grid represents the level grid, at a current time.
public class Grid : ICloneable
{
    // Store the width and height of the grid.
    readonly int width, height;
    
    readonly Box[] startBoxes;

    // Stores a 2D array of the box in every position in the grid.
    // Stores the index of the boxes in startBoxes, not the boxType!
    readonly int[,] grid;
    public Grid(int width, int height, ref Box[] startBoxes, bool isStationary = false) {
        this.width = width;
        this.height = height;
        this.startBoxes = startBoxes;
        // New grid
        grid = new int[width, height];
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                // Default: empty cell.
                grid[i, j] = -1;
            }
        }
        for (int i = 0; i < startBoxes.Length; i++) {    
            // Add to grid if it doesn't contain a box already.
            int boxType = (int) startBoxes[i].boxType;
            if (Constants.boxTypeProps[boxType].isIgnored) {
                continue;
            }
            if (!IsValidCoordinates(startBoxes[i].position)) {
                continue;
            }
            if (grid[startBoxes[i].position.x, startBoxes[i].position.y] != -1) {
                Debug.LogError("Grid already contains a box in that position!");
            } else if (Constants.boxTypeProps[boxType].isStationary == isStationary) {
                // Debug.Log(Constants.boxTypeNames[boxType]);
                grid[startBoxes[i].position.x, startBoxes[i].position.y] = i;
            }
        }
    }
    public Grid(int[,] grid, Box[] startBoxes) {
        width = grid.GetLength(0);
        height = grid.GetLength(1);
        this.startBoxes = startBoxes;
        this.grid = grid;
    }
    public object Clone()
    {
        int[,] clonedGrid = new int[width, height];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                clonedGrid[i, j] = grid[i, j];
            }
        }
        return new Grid(clonedGrid, startBoxes);
    }
    public bool IsCellEmpty((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return GetCellBoxType(coordinates) == (int) Constants.BoxTypes.Empty;
    }
    public bool IsCellGoal((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return GetCellBoxType(coordinates) == (int) Constants.BoxTypes.Goal;
    }

    public bool IsCellSublevel((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return GetCellBoxType(coordinates) == (int) Constants.BoxTypes.Sublevel;
    }
    public bool IsCellBlocked((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return Constants.boxTypeProps[GetCellBoxType(coordinates)].isBlockable;
    }

    public bool IsCellPushable((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return Constants.boxTypeProps[GetCellBoxType(coordinates)].isPushable;
    }

    public int GetCellBoxType((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return -1;
        }
        if (grid[coordinates.x, coordinates.y] == -1) {
            return (int) Constants.BoxTypes.Empty;
        }
        return (int) startBoxes[grid[coordinates.x, coordinates.y]].boxType;
    }

    bool IsValidCoordinates((int x, int y) coordinates) {
        bool isValid = coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x < width && coordinates.y < height;
        if (!isValid) {
            Debug.LogError("Validation of coordinates: Out of range!");
        }
        return isValid;
    }
    
    // Set by the startBoxesIndex
    public void SetCellInGrid((int x, int y) coordinates, int startBoxesIndex = -1) {
        if (IsValidCoordinates(coordinates)) {
            grid[coordinates.x, coordinates.y] = startBoxesIndex;
        }
    }

    // Render new grid after undo
    public void RenderNewGrid() {
        // Iterate through the grid and move the boxes into their positions.
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                if (grid[i, j] == -1) continue;
                startBoxes[grid[i, j]].SetPosition((i, j), this);
            }
        }
    }
}
