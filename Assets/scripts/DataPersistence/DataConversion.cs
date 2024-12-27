

//Obsolete
/*
using System.Collections.Generic;
using UnityEngine;

public class DataConversion {



    public static void AllLevelDataToSerializable(List<List<SavePlug>> levelsData,
                                           out List<List<Vector2>> levelPlugPositions,
                                           out List<List<bool>> levelPlugsArePluggedIn,
                                           out List<List<List<int>>> levelPlugPreviousCableIndices,
                                           out List<List<List<int>>> levelPlugCableDirections) { 
        levelPlugPositions            = new List<List<Vector2>>();
        levelPlugsArePluggedIn        = new List<List<bool>>();
        levelPlugPreviousCableIndices = new List<List<List<int>>>();
        levelPlugCableDirections      = new List<List<List<int>>>();

        foreach(List<SavePlug> levelSavePlug in levelsData) {
            List<Vector2>   tempPlugPositions;
            List<bool>      tempPlugsArePluggedIn;
            List<List<int>> tempPlugPreviousCableIndices;
            List<List<int>> tempPlugCableDirections;
            LevelPlugDataToSerializable(levelSavePlug, out tempPlugPositions, out tempPlugsArePluggedIn, out tempPlugPreviousCableIndices, out tempPlugCableDirections);
            levelPlugPositions.Add(tempPlugPositions);
            levelPlugsArePluggedIn.Add(tempPlugsArePluggedIn);
            levelPlugPreviousCableIndices.Add(tempPlugPreviousCableIndices);
            levelPlugCableDirections.Add(tempPlugCableDirections);
        }
    }
    public static void LevelPlugDataToSerializable(List<SavePlug> levelSavePlug, 
                                            out List<Vector2> plugPositions, 
                                            out List<bool> plugsArePluggedIn,
                                            out List<List<int>> plugPreviousCableIndices,
                                            out List<List<int>> plugCableDirections) {
        plugPositions            = new List<Vector2>();
        plugsArePluggedIn        = new List<bool>();
        plugPreviousCableIndices = new List<List<int>>();
        plugCableDirections      = new List<List<int>>();

        foreach(SavePlug savePlug in levelSavePlug) {
            Vector2   tempPlugPosition;
            bool      tempIsPluggedIn;
            List<int> tempPreviousCableIndices;
            List<int> tempCableDirections;

            PlugDataToSerializable(savePlug, out tempPlugPosition, out tempIsPluggedIn, out tempPreviousCableIndices, out tempCableDirections);
            plugPositions.Add(tempPlugPosition);
            plugsArePluggedIn.Add(tempIsPluggedIn);
            plugPreviousCableIndices.Add(tempPreviousCableIndices);
            plugCableDirections.Add(tempCableDirections);
        }
    }

    private static void PlugDataToSerializable(SavePlug savePlug, 
                                        out Vector2 plugPosition,
                                        out bool isPluggedIn,
                                        out List<int> previousCableIndices,
                                        out List<int> cableDirections) {
        previousCableIndices = new List<int>();
        cableDirections      = new List<int>();
        plugPosition = savePlug.plugPosition;
        isPluggedIn = savePlug.isPluggedIn;
        foreach(IndexAndDirection indexAndDirection in savePlug.indexAndDirections) {
            previousCableIndices.Add(indexAndDirection.previousIndex);
            switch(indexAndDirection.endingDirection) {
                case Directions.Up:
                cableDirections.Add(0);
                break;
                case Directions.Down:
                cableDirections.Add(1);
                break;
                case Directions.Left:
                cableDirections.Add(2);
                break;
                case Directions.Right:
                cableDirections.Add(3);
                break;
                default:
                Debug.LogError($"Unknown Direction detected in SerializePlugData: {indexAndDirection.endingDirection}");
                cableDirections.Add(0);
                break;
            }
        }
    }
}
*/