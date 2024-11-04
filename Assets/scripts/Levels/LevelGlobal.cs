using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelGlobal : LevelStartGlobal {
    public DebugC DebugC {set; get;}
    [SerializeField] private List<LevelPlugs> allLevelPlugs = new List<LevelPlugs>();
    private JointsData jointsData;

    
    // Start is called before the first frame update
    void Start() {
        DebugC = DebugC.Get();
        jointsData = FindObjectOfType<JointsData>();
        StartCoroutine(InitializeDelayed());
    }

    private IEnumerator InitializeDelayed() {
        yield return new WaitForSeconds(0.01f);
        RenewAllLevelPlugsList();
        SortAllLevelPlugs();
        RenewPlugSiblingIndices();
        MoveAllPlugsToInitialPositions();
        Log();
        base.FinishedWithAllTasks();
        //StartCoroutine(Test());
    }


    private void RenewAllLevelPlugsList() {
        allLevelPlugs.Clear();
        allLevelPlugs.Add(new LevelPlugs(Directions.Up));
        allLevelPlugs.Add(new LevelPlugs(Directions.Down));
        allLevelPlugs.Add(new LevelPlugs(Directions.Left));
        allLevelPlugs.Add(new LevelPlugs(Directions.Right));

        PlugAttributes[] plugAttributes = FindObjectsOfType<PlugAttributes>();
        foreach(PlugAttributes plugAttribute in plugAttributes) {
            if(plugAttribute.isObstacle) { continue; }
            Directions startingDirection = Utilities.TryGetComponentInChildren<CableChildAttributes>(plugAttribute.gameObject).startingDirection;
            switch(startingDirection) {
                case Directions.Up:
                    allLevelPlugs[0].plugAttributes.Add(plugAttribute);
                    break;
                case Directions.Down:
                    allLevelPlugs[1].plugAttributes.Add(plugAttribute);
                    break;
                case Directions.Left:
                    allLevelPlugs[2].plugAttributes.Add(plugAttribute);
                    break;
                case Directions.Right:
                    allLevelPlugs[3].plugAttributes.Add(plugAttribute);
                    break;
            }
        }
    }


    private void Log() {
        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            string text = "";
            foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                text += plugAttributes.name + ", ";
            }
            Directions direction = levelPlug.startingDirection;
            switch(direction) {
                case Directions.Up:
                    Debug.Log("Up: "+text);
                    break;
                case Directions.Down:
                    Debug.Log("Down: "+text);
                    break;
                case Directions.Left:
                    Debug.Log("Left: "+text);
                    break;
                case Directions.Right:
                    Debug.Log("Right: "+text);
                    break;
            }
        }
    }


    //Plugs are sorted in this order:
    //1. How tall the plug is 
    //2. How wide the plug is
    //Ex sort. [3x2, 3x1, 2x3, 2x2, 2x1, 1x1]
    private void SortAllLevelPlugs() {
        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            for(int i=0; i<levelPlug.plugAttributes.Count; i++) {
                PlugAttributes largestPlug = levelPlug.plugAttributes[i];
                int largestPlugIndex = i;
                for(int j=i+1; j<levelPlug.plugAttributes.Count; j++) {
                    if(levelPlug.plugAttributes[j].plugSize.x > largestPlug.plugSize.x ||
                       (levelPlug.plugAttributes[j].plugSize.x == largestPlug.plugSize.x && levelPlug.plugAttributes[j].plugSize.y > largestPlug.plugSize.y)) {
                        largestPlug = levelPlug.plugAttributes[j];
                        largestPlugIndex = j;
                    }
                }
                PlugAttributes temp = levelPlug.plugAttributes[i];
                levelPlug.plugAttributes[i] = levelPlug.plugAttributes[largestPlugIndex];
                levelPlug.plugAttributes[largestPlugIndex] = temp;
            }
        }
    }

    private void MoveAllPlugsToInitialPositions() {
        Transform [,] jointsGrid = jointsData.jointsGrid;

        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            Directions direction = levelPlug.startingDirection;
            Index2D index;
            switch(direction) {
                case Directions.Up:
                    index = new Index2D((int)Constants.startingPlugOffset.x-1, (int)Constants.startingPlugOffset.y-1);
                    //goes to the right
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x + Constants.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          jointsGrid[index.x, index.y].position.y - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,
                                                          jointsGrid[index.x, index.y].position.z);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        Debug.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x, index.y + (int)plugAttributes.plugSize.y);
                    }
                    break;
                case Directions.Down:
                    index = new Index2D(jointsGrid.GetLength(0)-(int)Constants.startingPlugOffset.x, (int)Constants.startingPlugOffset.y-1);
                    //goes to the right
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x + Constants.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          jointsGrid[index.x, index.y].position.y + (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,
                                                          jointsGrid[index.x, index.y].position.z);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        Debug.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x, index.y + (int)plugAttributes.plugSize.y);
                    }
                    break;
                case Directions.Left:
                    index = new Index2D((int)Constants.startingPlugOffset.x, (int)Constants.startingPlugOffset.y-1);
                    //goes down
                    for(int i=0; i<levelPlug.plugAttributes.Count; i++) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x,
                                                          jointsGrid[index.x, index.y].position.y - Constants.jointDistance*((levelPlug.plugAttributes[i].plugSize.x-1)/2) - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,//*(i+1),
                                                          jointsGrid[index.x, index.y].position.z);
                        levelPlug.plugAttributes[i].transform.position = newPosition - (Vector3)levelPlug.plugAttributes[i].center;
                        Debug.Log($"Plug {levelPlug.plugAttributes[i].name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x + (int)levelPlug.plugAttributes[i].plugSize.x, index.y);
                    }
                    break;
                case Directions.Right:
                    index = new Index2D((int)Constants.startingPlugOffset.x, jointsGrid.GetLength(1)-(int)Constants.startingPlugOffset.y);
                    //goes down
                    for(int i=0; i<levelPlug.plugAttributes.Count; i++) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x,
                                                          jointsGrid[index.x, index.y].position.y - Constants.jointDistance*((levelPlug.plugAttributes[i].plugSize.x-1)/2) - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,//*(i+1),
                                                          jointsGrid[index.x, index.y].position.z);
                        levelPlug.plugAttributes[i].transform.position = newPosition - (Vector3)levelPlug.plugAttributes[i].center;
                        Debug.Log($"Plug {levelPlug.plugAttributes[i].name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x + (int)levelPlug.plugAttributes[i].plugSize.x, index.y);
                    }
                    break;
            }
        }
        //Constant.startingPlugOffset;
    }

    private void RenewPlugSiblingIndices() {
        int siblingIndex = -1;
        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                siblingIndex++;
            }
        }
        for(int i=allLevelPlugs.Count-1; i>=0; i--) {
            for(int j=0; j<allLevelPlugs[i].plugAttributes.Count; j++) {
                allLevelPlugs[i].plugAttributes[j].transform.SetSiblingIndex(siblingIndex);
                siblingIndex--;
            }
        }
    }
}
