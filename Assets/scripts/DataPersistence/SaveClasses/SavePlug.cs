using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Will be deprecated soon
public class SavePlug {
    public Index2D socketIndex;
    public bool isPluggedIn;
    public List<IndexAndDirection> indexAndDirections;


    public SavePlug(Index2D socketIndex, bool isPluggedIn, List<IndexAndDirection> indexAndDirections) {
        this.socketIndex = socketIndex;
        this.isPluggedIn = isPluggedIn;
        this.indexAndDirections = indexAndDirections;
    }
    //Only use this constructor if isPluggedIn is false
    public SavePlug(bool isPluggedIn) {
        if(isPluggedIn) { Debug.LogWarning("The wrong SavePlug constructor was used. A plug that is plugged in should use the constructor with 3 parameters."); }
        this.isPluggedIn = isPluggedIn;
        this.socketIndex = new Index2D(-1, -1);
        this.indexAndDirections = new List<IndexAndDirection>();
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