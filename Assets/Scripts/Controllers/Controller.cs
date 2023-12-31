using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This manages the input controls
public class Controller : MonoBehaviour
{
    // The current level.
    Level currentLevel;

    // Set the current level
    public void SetCurrentLevel(Level level) {
        currentLevel = level;
    }

    // Update is called once per frame. Recieves the input from the player.
    void Update()
    {
        // If no current level, return.
        if (currentLevel == null) {
            Debug.LogError("No level set!");
            return;
        }

        if (Game.currentState != Game.GameStates.Ongoing) {
            return;
        }

        // If is current turn allow input, else wait until transition is over.
        if (currentLevel.isInTransition) {
            return;
        }

        if (Input.GetKeyDown(Constants.undo)) {
            currentLevel.Undo();
        }
        if (Input.GetKeyDown(Constants.restart)) {
            currentLevel.Restart();
        }

        // Can move
        if (Input.GetKeyDown(Constants.up)) {
            MoveCurrentPosition(Constants.moveUp);
    
        } else if (Input.GetKeyDown(Constants.down)) {
            MoveCurrentPosition(Constants.moveDown);
    
        } else if (Input.GetKeyDown(Constants.right)) {
            MoveCurrentPosition(Constants.moveRight);
    
        } else if (Input.GetKeyDown(Constants.left)) {
            MoveCurrentPosition(Constants.moveLeft);
    
        }
    }
    void MoveCurrentPosition((int x, int y) direction) {
        // Update the active box
        if (!currentLevel.MoveControlledBox(direction)) {
            return;
        }

        // Wait until no active transitions before moving the player
        // Move the player if the active box really moves
        StartCoroutine(currentLevel.MovePlayer());
    }
}
