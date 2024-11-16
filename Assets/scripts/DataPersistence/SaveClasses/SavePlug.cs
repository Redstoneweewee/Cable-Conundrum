using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlug {
    public Vector3 plugPosition;
    public bool isPluggedIn;

    public SavePlug(Vector3 plugPosition, bool isPluggedIn) {
        this.plugPosition = plugPosition;
        this.isPluggedIn = isPluggedIn;
    }
}
