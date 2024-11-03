using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelPlugs {
    public Directions startingDirection;
    public List<Plug> plugs;
    public LevelPlugs(Directions startingDirection) {
        this.startingDirection = startingDirection;
        plugs = new List<Plug>();
    }
}
