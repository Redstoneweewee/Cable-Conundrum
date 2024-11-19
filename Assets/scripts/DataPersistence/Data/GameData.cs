using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

    /*
    * levelSavePlugs[i] = specific level, List<SavePlug>
    * levelSavePlugs[i][j] = specific plug in the level SavePlug
    */
    public List<List<SavePlug>> levelsSavePlugs;
    public List<bool> levelCompletion;

    //Default values when the game first starts
    public GameData() {
        levelsSavePlugs = new List<List<SavePlug>>();
        levelCompletion = new List<bool>();
    }
}
