using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class includes helper functions for a grid
public static class GridHelper
{
    // Calculate the direction tuple, given two positions tuples.
    public static (int x, int y) CalcDirection((int x, int y) initPosition, (int x, int y) finalPosition) {
        return (finalPosition.x - initPosition.x, finalPosition.y - initPosition.y);
    }
}
