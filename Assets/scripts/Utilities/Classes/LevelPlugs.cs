using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelPlugs {
    public Directions startingDirection;
    public List<PlugAttributes> plugAttributes;
    public LevelPlugs(Directions startingDirection) {
        this.startingDirection = startingDirection;
        plugAttributes = new List<PlugAttributes>();
    }
}
