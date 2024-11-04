using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelController : LevelStart {
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

    private IEnumerator Test() {
        yield return new WaitForSeconds(1f);
        base.FinishedWithAllTasks();
    }

    private void RenewAllLevelPlugsList() {
        allLevelPlugs.Clear();
        allLevelPlugs.Add(new LevelPlugs(Directions.Up));
        allLevelPlugs.Add(new LevelPlugs(Directions.Down));
        allLevelPlugs.Add(new LevelPlugs(Directions.Left));
        allLevelPlugs.Add(new LevelPlugs(Directions.Right));

        Plug[] plugs = FindObjectsOfType<Plug>();
        foreach(Plug plug in plugs) {
            if(plug.isObstacle) { continue; }
            Directions startingDirection = Utilities.TryGetComponentInChildren<CableChildAttributes>(plug.gameObject).startingDirection;
            switch(startingDirection) {
                case Directions.Up:
                    allLevelPlugs[0].plugs.Add(plug);
                    break;
                case Directions.Down:
                    allLevelPlugs[1].plugs.Add(plug);
                    break;
                case Directions.Left:
                    allLevelPlugs[2].plugs.Add(plug);
                    break;
                case Directions.Right:
                    allLevelPlugs[3].plugs.Add(plug);
                    break;
            }
        }
    }


    private void Log() {
        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            string text = "";
            foreach(Plug plug in levelPlug.plugs) {
                text += plug.name + ", ";
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
            for(int i=0; i<levelPlug.plugs.Count; i++) {
                Plug largestPlug = levelPlug.plugs[i];
                int largestPlugIndex = i;
                for(int j=i+1; j<levelPlug.plugs.Count; j++) {
                    if(levelPlug.plugs[j].plugSize.x > largestPlug.plugSize.x ||
                       (levelPlug.plugs[j].plugSize.x == largestPlug.plugSize.x && levelPlug.plugs[j].plugSize.y > largestPlug.plugSize.y)) {
                        largestPlug = levelPlug.plugs[j];
                        largestPlugIndex = j;
                    }
                }
                Plug temp = levelPlug.plugs[i];
                levelPlug.plugs[i] = levelPlug.plugs[largestPlugIndex];
                levelPlug.plugs[largestPlugIndex] = temp;
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
                    foreach(Plug plug in levelPlug.plugs) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x + Constants.jointDistance*((plug.plugSize.y-1)/2),
                                                          jointsGrid[index.x, index.y].position.y - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,
                                                          jointsGrid[index.x, index.y].position.z);
                        plug.transform.position = newPosition - (Vector3)plug.center;
                        Debug.Log($"Plug {plug.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x, index.y + (int)plug.plugSize.y);
                    }
                    break;
                case Directions.Down:
                    index = new Index2D(jointsGrid.GetLength(0)-(int)Constants.startingPlugOffset.x, (int)Constants.startingPlugOffset.y-1);
                    //goes to the right
                    foreach(Plug plug in levelPlug.plugs) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x + Constants.jointDistance*((plug.plugSize.y-1)/2),
                                                          jointsGrid[index.x, index.y].position.y + (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,
                                                          jointsGrid[index.x, index.y].position.z);
                        plug.transform.position = newPosition - (Vector3)plug.center;
                        Debug.Log($"Plug {plug.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x, index.y + (int)plug.plugSize.y);
                    }
                    break;
                case Directions.Left:
                    index = new Index2D((int)Constants.startingPlugOffset.x, (int)Constants.startingPlugOffset.y-1);
                    //goes down
                    for(int i=0; i<levelPlug.plugs.Count; i++) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x,
                                                          jointsGrid[index.x, index.y].position.y - Constants.jointDistance*((levelPlug.plugs[i].plugSize.x-1)/2) - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,//*(i+1),
                                                          jointsGrid[index.x, index.y].position.z);
                        levelPlug.plugs[i].transform.position = newPosition - (Vector3)levelPlug.plugs[i].center;
                        Debug.Log($"Plug {levelPlug.plugs[i].name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x + (int)levelPlug.plugs[i].plugSize.x, index.y);
                    }
                    break;
                case Directions.Right:
                    index = new Index2D((int)Constants.startingPlugOffset.x, jointsGrid.GetLength(1)-(int)Constants.startingPlugOffset.y);
                    //goes down
                    for(int i=0; i<levelPlug.plugs.Count; i++) {
                        Vector3 newPosition = new Vector3(jointsGrid[index.x, index.y].position.x,
                                                          jointsGrid[index.x, index.y].position.y - Constants.jointDistance*((levelPlug.plugs[i].plugSize.x-1)/2) - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,//*(i+1),
                                                          jointsGrid[index.x, index.y].position.z);
                        levelPlug.plugs[i].transform.position = newPosition - (Vector3)levelPlug.plugs[i].center;
                        Debug.Log($"Plug {levelPlug.plugs[i].name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x + (int)levelPlug.plugs[i].plugSize.x, index.y);
                    }
                    break;
            }
        }
        //Constant.startingPlugOffset;
    }

    private void RenewPlugSiblingIndices() {
        int siblingIndex = -1;
        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            foreach(Plug plug in levelPlug.plugs) {
                siblingIndex++;
            }
        }
        for(int i=allLevelPlugs.Count-1; i>=0; i--) {
            for(int j=0; j<allLevelPlugs[i].plugs.Count; j++) {
                allLevelPlugs[i].plugs[j].transform.SetSiblingIndex(siblingIndex);
                siblingIndex--;
            }
        }
    }
}
