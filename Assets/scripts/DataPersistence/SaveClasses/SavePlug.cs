using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePlug {
    public Index2D socketIndex;
    public bool isPluggedIn;
    public List<IndexAndDirection> indexAndDirections;


    public SavePlug(Index2D socketIndex, bool isPluggedIn, List<IndexAndDirection> indexAndDirections) {
        this.socketIndex = socketIndex;
        this.isPluggedIn = isPluggedIn;
        this.indexAndDirections = indexAndDirections;
    }
    public SavePlug(bool isPluggedIn, List<IndexAndDirection> indexAndDirections) {
        this.socketIndex = new Index2D(-1, -1);
        this.isPluggedIn = isPluggedIn;
        this.indexAndDirections = indexAndDirections;
    }
    public SavePlug() {
        this.socketIndex = new Index2D(-1, -1);
        this.isPluggedIn = false;
        this.indexAndDirections = new List<IndexAndDirection>();
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