using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

    /*
    * levelSavePlugs[i] = specific level, List<SavePlug>
    * levelSavePlugs[i][j] = specific plug in the level SavePlug
    */
    public SaveSettings settings;
    public List<List<SavePlug>> levelsSavePlugs;
    public List<bool> levelCompletion;

    //Default values when the game first starts
    public GameData() {
        levelsSavePlugs = new List<List<SavePlug>>();
        levelCompletion = new List<bool>();
        settings        = new SaveSettings();
    }


    public void Log() {
        string output;
        output = "gamedata data:";
        output += $"\nsettings:\n"+
                  $"sound volume: {settings.soundVolume}\n"+
                  $"music volume: {settings.musicVolume}\n";
        output += "levelCompletion: [";
        foreach(bool completion in levelCompletion) {
            output += completion + ", ";
        }
        output += "]\n";
        int level = 1;
        foreach(List<SavePlug> list in levelsSavePlugs) {
            output += "level "+level+" data:\n";
            foreach(SavePlug savePlug in list) {
                output += $"   socketIndex: {savePlug.socketIndex}, "+
                          $"isPluggedIn: {savePlug.isPluggedIn}";
                if(savePlug.indexAndDirections != null) {
                    foreach(IndexAndDirection indexAndDirection in savePlug.indexAndDirections) {
                        output += $"\n      IandD prev: {indexAndDirection.previousIndex}, "+
                                $"IandD endDir: {indexAndDirection.endingDirection}";
                    }
                }
                output += "\n";
            }
            level++;
        }
        Debug.Log(output);
    }
}


//Obsolete
/*
[System.Serializable]
public class GameDataSerializable {
    //public List<List<Vector2>>   levelPlugPositions;
    public List<bool>      levelPlugsArePluggedIn;
    //public List<List<List<int>>> levelPlugPreviousCableIndices;
    //public List<List<List<int>>> levelPlugCableDirections;

    public GameDataSerializable(GameData gameData) {
        //Convert GameData into serializable form
        //levelPlugPositions            = new List<List<Vector2>>();
        levelPlugsArePluggedIn        = new List<bool>();
        //levelPlugPreviousCableIndices = new List<List<List<int>>>();
        //levelPlugCableDirections      = new List<List<List<int>>>();
        levelPlugsArePluggedIn = new List<bool>{true, false, true, false};
        //DataConversion.AllLevelDataToSerializable(gameData.levelsSavePlugs, out levelPlugPositions, out levelPlugsArePluggedIn,
        //                                            out levelPlugPreviousCableIndices, out levelPlugCableDirections);
        Debug.Log("testings: " +levelPlugsArePluggedIn[0]??"none");
    }
}
*/