using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

// Grid represents the level grid, at a current time.
public class Grid : ICloneable
{
    // Store the width and height of the grid.
    readonly int width, height;

    // Stores a 2D array of the box in every position in the grid.
    readonly int[,] grid;
    public Grid(int width, int height, ref Box[] startBoxes, bool isStationary = false) {
        this.width = width;
        this.height = height;
        // New grid
        grid = new int[width, height];

        foreach (Box box in startBoxes) {
            // Add to grid if it doesn't contain a box already.
            int boxType = (int) box.boxType;
            if (Constants.boxTypeProps[boxType].isIgnored) {
                continue;
            }
            if (!IsValidCoordinates(box.position)) {
                continue;
            }
            if (grid[box.position.x, box.position.y] != 0) {
                Debug.LogError("Grid already contains a box in that position!");
            } else if (Constants.boxTypeProps[boxType].isStationary == isStationary) {
                Debug.Log(Constants.boxTypeNames[boxType]);
                grid[box.position.x, box.position.y] = boxType;
            }
        }
    }
    public Grid(int[,] grid) {
        width = grid.GetLength(0);
        height = grid.GetLength(1);
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
        return new Grid(clonedGrid);
    }
    public bool IsCellEmpty((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return grid[coordinates.x, coordinates.y] == (int) Constants.BoxTypes.Empty;
    }
    public bool IsCellGoal((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return grid[coordinates.x, coordinates.y] == (int) Constants.BoxTypes.Goal;
    }

    public bool IsCellSublevel((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return grid[coordinates.x, coordinates.y] == (int) Constants.BoxTypes.Sublevel;
    }

    public bool IsCellBlocked((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return Constants.boxTypeProps[grid[coordinates.x, coordinates.y]].isBlockable;
    }

    public bool IsCellPushable((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return false;
        }
        return Constants.boxTypeProps[grid[coordinates.x, coordinates.y]].isPushable;
    }

    public int GetCellBoxType((int x, int y) coordinates) {
        if (!IsValidCoordinates(coordinates)) {
            return -1;
        }
        return grid[coordinates.x, coordinates.y];
    }

    bool IsValidCoordinates((int x, int y) coordinates) {
        bool isValid = coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x < width && coordinates.y < height;
        if (!isValid) {
            Debug.LogError("Validation of coordinates: Out of range!");
        }
        return isValid;
    }
    
    public void SetCellInGrid((int x, int y) coordinates, Constants.BoxTypes boxType) {
        if (IsValidCoordinates(coordinates)) {
            grid[coordinates.x, coordinates.y] = (int) boxType;
        }
    }
}
