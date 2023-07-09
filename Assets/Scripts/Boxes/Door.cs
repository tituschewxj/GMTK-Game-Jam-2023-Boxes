using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door
{
    readonly List<Box> buttons; // Store all the buttons
    public Door(ref Box[] startBoxes) {
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

    public static void OpenDoor(ref Box box, Grid staticGrid, bool force = false) {
        if (!force && box.boxType != Constants.BoxTypes.ClosedDoor) {
            Debug.LogWarning("Invalid: Not closed door, cannot open");
        }
        box.boxType = Constants.BoxTypes.OpenedDoor;
        staticGrid.SetCellInGrid(box.position, Constants.BoxTypes.OpenedDoor);
    }

    public static void CloseDoor(ref Box box, Grid staticGrid, bool force = false) {
        if (!force && box.boxType != Constants.BoxTypes.OpenedDoor) {
            Debug.LogWarning("Invalid: Not open door, cannot close");
        }
        box.boxType = Constants.BoxTypes.ClosedDoor;
        staticGrid.SetCellInGrid(box.position, Constants.BoxTypes.ClosedDoor);
    }

    public void UpdateDoors(LevelHistory levelHistory) {
        for (int i = 0; i < buttons.Count; i++) {
            // Use the dynamic grid to detect if there are any buttons pressed
            Box door = buttons[i].otherBox;
            if (levelHistory.currentGrid.GetCellBoxType(buttons[i].position) != (int) Constants.BoxTypes.Empty) {
                // Trigger button update on the static grid
                OpenDoor(ref door, levelHistory.staticGrid, force: true);
            } else {
                // Trigger button update on the static grid
                CloseDoor(ref door, levelHistory.staticGrid, force: true);
            }
        }
    }
}
