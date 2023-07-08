using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A box is something that represents a something in a cell, that is not empty.
// A box will hold a sprite that represents itself.
public class Box: MonoBehaviour
{
    // Coordinates of the box in the grid.
    public (int x, int y) position;
    // The type of box.
    public Constants.BoxTypes boxType;
    // Represents the active box that is being controlled currently.
    public bool isActiveBox;
    void Awake() {
        position.x = (int) transform.position.x;
        position.y = (int) transform.position.y;
        if (position.x < 0 || position.y < 0) {
            Debug.LogErrorFormat("Box coordinates is out of range! Set x and y to be non-negative.");
        }
    }

    // Moves the box in a direction
    public void MoveBox((int x, int y) direction, Grid currentGrid = null) {
        currentGrid?.SetCellInGrid(position, Constants.BoxTypes.Empty);
        
        // Move the current position
        position.x += direction.x;
        position.y += direction.y;
        transform.position = new(position.x, position.y);
    
        currentGrid?.SetCellInGrid(position, boxType);
    }

    public void SetPosition((int x, int y) position, Grid currentGrid = null) {
        currentGrid?.SetCellInGrid(this.position, Constants.BoxTypes.Empty);

        this.position = position;
        transform.position = new(position.x, position.y);

        currentGrid?.SetCellInGrid(position, boxType);

    }

    public bool IsAtCoordinates((int x, int y) coordinates) {
        return coordinates == position;
    }
}
