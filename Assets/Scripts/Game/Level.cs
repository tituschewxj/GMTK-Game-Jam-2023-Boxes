using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    // Stores the width and height of the level.
    public int width, height;

    // Store the starting boxes
    Box[] startBoxes; // TODO: refactor to just store movable boxes.

    // Stores history related information of the level
    LevelHistory levelHistory;
    Controller controller;

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
    UI ui;
    SceneLoader sceneLoader;
    DoorHelper doorManager;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get the initial state of the level, startBoxes should not be modified:
        startBoxes = gameObject.GetComponentsInChildren<Box>();
        // Assign the startBoxes with their index.
        for (int i = 0; i <startBoxes.Length; i++) {
            startBoxes[i].index = i;
        }

        // Find the active box:
        foreach (Box box in startBoxes) {
            if (box.isActiveBox) {
                if (activeBox != null) {
                    Debug.LogError("Cannot have multiple active boxes!");
                    break;
                }
                activeBox = box;
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

        // Find UI
        ui = FindObjectOfType<UI>();
        if (ui == null) {
            Debug.LogError("No UI found!");
        }

        // Find SceneLoader
        sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader == null) {
            Debug.LogError("No sceneLoader found!");
        }
    
        // Find audioManager
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null) {
            Debug.LogError("No audioManager found!");
        }

        // Find doors
        doorManager = new(ref startBoxes);

        // Update game state:
        Game.currentState = Game.GameStates.Ongoing;

        // New history for level:
        levelHistory = new(width, height, ref startBoxes, activeBox.position, player);
        doorManager.UpdateDoors(levelHistory);
    }


    // Moves the box that is currently controlled
    // Returns whether the box really moves
    public bool MoveControlledBox((int x, int y) direction) {
        (int x, int y) newPosition = levelHistory.currentPosition;
        newPosition.x += direction.x;
        newPosition.y += direction.y;

        if (levelHistory.IsOtherControllableBox(newPosition)) {
            // move the active position only and switch the active box.
            audioManager.PlayBoxswitch();
            UpdateCurrentPosition(newPosition);
            SwitchActiveBox(newPosition);
            return false;
        }
        
        // Validation
        if (levelHistory.IsCellBlocked(newPosition)) {
            audioManager.PlayCannot();
            return false;
        }

        // Update history at the start of every turn (which the player can move)
        levelHistory.PushToHistory();

        // Move box and update grid
        audioManager.PlayBoxmove();
        UpdateCurrentPosition(newPosition);
        activeBox.MoveBox(direction, levelHistory.currentGrid);

        // Check if sublevel
        if (levelHistory.AtSublevelPortal()) {
            // load sublevel based on otherProperty of the box found
            audioManager.PlaySublevel();
            Game.currentState = Game.GameStates.Transition;
            Box b = BoxHelper.GetBox(levelHistory.currentPosition, ref startBoxes);
            ui.LoadingSublevelTransition(() => StartCoroutine(sceneLoader.LoadSublevelIndex(b.otherProperty)));
        }

        // Trigger any button interactions
        doorManager.UpdateDoors(levelHistory);
        return true;
    }

    // Switches the actively controlled box.
    public void SwitchActiveBox((int x, int y) coordinates) {
        // Find new active box:
        for (int i = 0; i < startBoxes.Length; i++) {
            if (startBoxes[i].IsAtCoordinates(coordinates) && Constants.boxTypeProps[(int) startBoxes[i].boxType].isControllable) {
                activeBox.isActiveBox = false;
                startBoxes[i].isActiveBox = true;
                activeBox = BoxHelper.GetBoxByIndex(i, ref startBoxes);
                UpdateCurrentPosition(coordinates);
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
            while (levelHistory.IsCellPushable(newPosition)) {
                newPosition = player.MoveInDirection(newPosition);
            }
            if (levelHistory.IsCellBlocked(newPosition)) {
                // cannot move in that direction if blocked, switch direction.
                player.SwitchDirection();
                newPosition = player.GetNewPosition();
                continue;
            } 

            // push all boxes in the line if any
            while (newPosition != player.GetNewPosition() ) {
                // TODO: REFACTOR: push box
                // The order of the pushing has to be from the end
                (int x, int y) prevPosition = player.MoveInDirection(newPosition, inverse: true);
                for (int j = 0; j < startBoxes.Length; j++) {
                    if (startBoxes[j].IsAtCoordinates(prevPosition) && 
                        Constants.boxTypeProps[(int) startBoxes[j].boxType].isPushable) {
                        startBoxes[j].MoveToPosition(newPosition, levelHistory.currentGrid);
                        break;
                    }
                }
                newPosition = prevPosition;
            }
            // can move
            audioManager.PlayPlayermove();
            player.MovePlayer(levelHistory.currentGrid);
            break;
        }

        // Trigger any button interactions
        doorManager.UpdateDoors(levelHistory);

        // Check if goal reached!
        if (player.IsAtGoal(levelHistory)) {
            audioManager.PlayGoal();
            ui.LevelCompleteTransition(() => StartCoroutine(sceneLoader.LoadPreviousScene()));
        }
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

    public void UpdateCurrentPosition((int x, int y) coordinates) {
        levelHistory.UpdateCurrentPosition(coordinates);
    }
    public void Undo() {
        if (!levelHistory.Undo()) {
            audioManager.PlayCannot();
            return;
        }
        audioManager.PlayUndo();
        Rerender();
    }

    public void Restart() {
        levelHistory.Clear();
        audioManager.PlayUndo();
        Rerender();
    }
    void Rerender() {
        player.SetDirectionByIndex(levelHistory.currentPlayerDirection);

        // Get new active box, using the currentPosition:
        cursor.SetPosition(levelHistory.currentPosition);
        activeBox.isActiveBox = false;
        int i = BoxHelper.GetBoxIndex(levelHistory.currentPosition, ref startBoxes);
        activeBox = BoxHelper.GetBoxByIndex(i, ref startBoxes);
        activeBox.isActiveBox = true;
        
        doorManager.UpdateDoors(levelHistory);
    }
}
