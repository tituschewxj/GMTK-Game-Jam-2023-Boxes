using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHelper
{
    readonly List<Box> buttons; // Store all the buttons
    public DoorHelper(ref Box[] startBoxes) {
        buttons = new();
        // iterate through startBoxes and add doors and buttons
        for (int i = 0; i < startBoxes.Length; i++) {
            if (startBoxes[i].boxType == Constants.BoxTypes.Button) {
                buttons.Add(startBoxes[i]);
                // Link button and doors: button.otherProperty represents the door in the array of boxTypes.
                if (startBoxes[i].otherBox.boxType != Constants.BoxTypes.OpenedDoor &&
                    startBoxes[i].otherBox.boxType != Constants.BoxTypes.ClosedDoor) {
                    Debug.LogWarning("Invalid door in otherBox property!");
                }
            }
        }
    }

    public static void OpenDoor(ref Box door, Grid staticGrid, bool force = false) {
        if (!force && door.boxType != Constants.BoxTypes.ClosedDoor) {
            Debug.LogWarning("Invalid: Not closed door, cannot open");
        }
        if (door.boxType == Constants.BoxTypes.ClosedDoor) {
            door.doorManager.OpenDoorTransition();
        }
        door.boxType = Constants.BoxTypes.OpenedDoor;
        staticGrid.SetCellInGrid(door.position, door.index);
    }

    public static void CloseDoor(ref Box door, Grid staticGrid, bool force = false) {
        if (!force && door.boxType != Constants.BoxTypes.OpenedDoor) {
            Debug.LogWarning("Invalid: Not open door, cannot close");
        }
        if (door.boxType == Constants.BoxTypes.OpenedDoor) {
            door.doorManager.CloseDoorTransition();
        }
        door.boxType = Constants.BoxTypes.ClosedDoor;
        staticGrid.SetCellInGrid(door.position, door.index);
    }

    public void UpdateDoors(LevelHistory levelHistory) {
        for (int i = 0; i < buttons.Count; i++) {
            // Use the dynamic grid to detect if there are any buttons pressed
            Box door = buttons[i].otherBox;
            if (levelHistory.currentGrid.GetCellBoxType(buttons[i].position) != (int) Constants.BoxTypes.Empty) {
                // Trigger button update on the static grid
                OpenDoor(ref door, levelHistory.staticGrid, force: true);
            } else {
                // Ensure that the space is empty before closing the door
                if (levelHistory.currentGrid.GetCellBoxType(door.position) == (int) Constants.BoxTypes.Empty) {
                    // Trigger button update on the static grid
                    CloseDoor(ref door, levelHistory.staticGrid, force: true);
                }
            }
        }
    }
}
