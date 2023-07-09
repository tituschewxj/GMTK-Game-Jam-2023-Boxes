using UnityEngine;

public static class BoxHelper {
    public static Box GetBox((int x, int y) coordinates, ref Box[] boxes) {
        for (int i = 0; i < boxes.Length; i++) {
            if (boxes[i].position == coordinates && !Constants.boxTypeProps[(int) boxes[i].boxType].isIgnored) {
                return boxes[i];
            }
        }
        Debug.LogWarning("GetBox: No box found!");
        return null;
    }
    public static int GetBoxIndex((int x, int y) coordinates, ref Box[] boxes) {
        for (int i = 0; i < boxes.Length; i++) {
            if (boxes[i].position == coordinates && !Constants.boxTypeProps[(int) boxes[i].boxType].isIgnored) {
                return i;
            }
        }
        Debug.LogWarning("GetBox: No box found!");
        return -1;
    }

    public static Box GetBoxByType((int x, int y) coordinates, Constants.BoxTypes type, ref Box[] boxes) {
        for (int i = 0; i < boxes.Length; i++) {
            if (boxes[i].position == coordinates && boxes[i].boxType == type) {
                return boxes[i];
            }
        }
        Debug.LogWarning("GetBox: No box found!");
        return null;
    }

    public static ref Box GetBoxByIndex(int index, ref Box[] boxes) {
        return ref boxes[index];
    }
}