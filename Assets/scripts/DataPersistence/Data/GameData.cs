using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public int testingInt;

    /*
    * levelSavePlugs[i] = specific level, List<SavePlug>
    * levelSavePlugs[i][j] = specific plug in the level SavePlug
    */
    public List<List<SavePlug>> levelsSavePlugs;
    public List<bool> levelCompletion;

    //Default values when the game first starts
    public GameData() {
        this.testingInt = 0;
        //levelsSavePlugs = new List<List<SavePlug>>(new List<SavePlug>[Constants.numberOfLevels]);
        levelsSavePlugs = new List<List<SavePlug>>();
        levelCompletion = new List<bool>();
    }
}
