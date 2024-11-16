using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public int testingInt;

    // TODO - store plugs' position, cablesAttributes (all of them), and isPluggedIn
    public SavePlug testingSavePlug;

    //Defalut values when the game first starts
    public GameData() {
        this.testingInt = 0;
    }
}
