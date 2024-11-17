using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePlug {
    public Vector3 plugPosition;
    public bool isPluggedIn;
    public List<IndexAndDirection> indexAndDirections;


    public SavePlug(Vector3 plugPosition, bool isPluggedIn, List<IndexAndDirection> indexAndDirections) {
        this.plugPosition = plugPosition;
        this.isPluggedIn = isPluggedIn;
        this.indexAndDirections = indexAndDirections;
    }
}

public struct IndexAndDirection {
    public int previousIndex;
    public Directions endingDirection;

    public IndexAndDirection(int previousIndex, Directions endingDirection) {
        this.previousIndex = previousIndex;
        this.endingDirection = endingDirection;
    }
}