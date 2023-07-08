using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This represents the player box, which moves after a box moves from the input.
public class Player
{
    int directionIndex = 0;
    public (int x, int y) currentDirection;
    readonly Box box;

    public Player(Box box) {
        this.box = box;
        currentDirection =  Constants.directions[directionIndex];
    }

    // Moves the player in the current direction
    public void MovePlayer(Grid currentGrid = null) {
        box.MoveBox(currentDirection, currentGrid);
    }
    public (int x, int y) GetPosition() {
        return box.position;
    }
    public (int x, int y) GetDirection() {
        return currentDirection;
    }

    // Gets the new position if the player moves in the current direction
    public (int x, int y) GetNewPosition() {
        (int x, int y) newPosition = box.position;
        newPosition.x += currentDirection.x;
        newPosition.y += currentDirection.y;
        return newPosition;
    }

    // Gets the position in the opposite of the current direction, assuming that the direction didn't change.
    public (int x, int y) GetPrevPosition() {
        (int x, int y) newPosition = box.position;
        newPosition.x -= currentDirection.x;
        newPosition.y -= currentDirection.y;
        return newPosition;
    }

    // Returns the new position in that direction, or inversed.
    public (int x, int y) MoveInDirection((int x, int y) position, bool inverse = false) {
        position.x += currentDirection.x * (inverse ? -1 : 1);
        position.y += currentDirection.y * (inverse ? -1 : 1);
        return position;
    }
    public void SwitchDirection() {
        directionIndex = (directionIndex + 1) % Constants.directions.Length;
        currentDirection = Constants.directions[directionIndex];
    }
}
