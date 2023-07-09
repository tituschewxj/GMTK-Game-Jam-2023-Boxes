using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A box is something that represents a something in a cell, that is not empty.
// A box will hold a sprite that represents itself.
public class Box: MonoBehaviour
{
    Level level;
    // Index of the box in the startBoxes array.
    public int index;
    // Coordinates of the box in the grid.
    public (int x, int y) position;
    // The type of box.
    public Constants.BoxTypes boxType;
    // Represents the active box that is being controlled currently.
    public bool isActiveBox;
    // Represents the an other property of the box, e.g.: sublevel index or button toggle door index, etc. 
    // Can be unused: -1
    public int otherProperty = -1;
    // If another box is linked to this box, e.g.: the door related to the button.
    public Box otherBox;
    // If updates to the door needs to be made for that box, else null.
    public Door doorManager;
    void Awake() {
        position.x = (int) transform.position.x;
        position.y = (int) transform.position.y;
        if (position.x < 0 || position.y < 0) {
            Debug.LogErrorFormat("Box coordinates is out of range! Set x and y to be non-negative.");
        }
    }
    void Start() {
        level = gameObject.GetComponentInParent<Level>();
    }

    // Moves the box in a direction, with tweening.
    public void MoveBox((int x, int y) direction, Grid currentGrid = null) {
        // By default, it assumes that nothing is in that cell when it leaves, so the order of box movement matters.
        currentGrid?.SetCellInGrid(position);
        
        // Move the current position
        position.x += direction.x;
        position.y += direction.y;
    
        // Tween
        level.StartMovement();
        LeanTween.move(gameObject, new Vector2(position.x, position.y), Constants.transitionTime).
            setOnComplete(level.FinishMovement);
        
        // If active box, also move cursor and set new currentPosition. There is only one active box.
        if (isActiveBox) {
            level.cursor.MoveBox(direction);
            level.UpdateCurrentPosition(position);
        }

        currentGrid?.SetCellInGrid(position, index);
    }

    // Moves the box the a position, with tweening.
    public void MoveToPosition((int x, int y) position, Grid currentGrid = null) {
        MoveBox(GridHelper.CalcDirection(this.position, position), currentGrid);
    }

    // This doesn't use tweening!
    public void SetPosition((int x, int y) position, Grid currentGrid = null) {
        currentGrid?.SetCellInGrid(this.position);

        this.position = position;
        transform.position = new(position.x, position.y);

        // If active box, also move cursor
        if (isActiveBox) {
            level.cursor.SetPosition(position);
            level.UpdateCurrentPosition(position);
        }

        currentGrid?.SetCellInGrid(position, index);
    }

    public bool IsAtCoordinates((int x, int y) coordinates) {
        return coordinates == position;
    }
}
