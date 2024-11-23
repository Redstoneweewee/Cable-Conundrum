using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelInitializerGlobal : InitializerBase, IDataPersistence {
    public DebugC DebugC {set; get;}
    public int levelIndex;
    [HideInInspector] private List<LevelPlugs> allLevelPlugs = new List<LevelPlugs>();
    [HideInInspector] private List<PlugAttributes> sortedPlugs = new List<PlugAttributes>();
    private GridsSkeleton gridsSkeleton;
    private bool initializationFinished = false;
    //public bool allCableHandlersInitializationFinished = false;



    // |----------------------------------------------------------------------------------|
    // |----------------------------------------------------------------------------------|
    // |----- Save and LoadData does NOT work with PlugSelector dragged in plugs!!!! -----|
    // |----------------------------------------------------------------------------------|
    // |----------------------------------------------------------------------------------|
    
    //TryGenerateCableFromList
    public IEnumerator LoadData(GameData data) {
        yield return new WaitUntil(() => initializationFinished);
        //yield return new WaitUntil(() => allCableHandlersInitializationFinished);

        if(data.levelsSavePlugs[levelIndex] != null) {
            List<SavePlug> levelData = data.levelsSavePlugs[levelIndex];
            //Set all the necessary data for a plug
            for(int i=0; i<levelData.Count; i++) {
                if(levelData[i] == null) { continue; }
                if(sortedPlugs.Count <= i) { continue; }

                SavePlug savePlug = levelData[i];
                GameObject plug = sortedPlugs[i].gameObject;
                Debug.Log($"Inheriting values - receiver: {plug.name}");
                Debug.Log($"plugPosition changed: {plug.name} from ({plug.transform.position}) to ({savePlug.plugPosition})");
                plug.transform.position = savePlug.plugPosition;
                Debug.Log($"isPluggedIn changed: {plug.name} from ({Utilities.TryGetComponent<PlugAttributes>(plug).isPluggedIn}) to ({savePlug.isPluggedIn})");
                Utilities.TryGetComponent<PlugAttributes>(plug).isPluggedIn = savePlug.isPluggedIn;

                if(savePlug.indexAndDirections == null) { continue; }
                CableHandler cableHandler = Utilities.TryGetComponentInChildren<CableHandler>(plug);
                cableHandler.TryGenerateCableFromList(savePlug.indexAndDirections);
            }
        }

        //Once all data is loaded, we are finished with all tasks
        base.FinishedWithAllTasks();
        //And renew level grids
        FindObjectOfType<GridsController>().RenewGrids();
    }

    
    public void SaveData(GameData data) {
        //If the level has already been completed, do not save any new changes
        if(data.levelCompletion[levelIndex]) { return; }

        data.levelsSavePlugs[levelIndex].Clear();
        DebugC.Get().LogListAlways("level savePlug: ", data.levelsSavePlugs[levelIndex]);
        foreach(PlugAttributes plugAttribute in sortedPlugs) {
            GameObject plug = plugAttribute.gameObject;
            Vector3 plugPosition = plug.transform.position;
            bool isPluggedIn = plugAttribute.isPluggedIn;

            List<IndexAndDirection> indexAndDirections = new List<IndexAndDirection>();
            
            int startingIndex = plugAttribute.cableParentAttributes.initialCables.Count;
            CableParentAttributes parentAttributes = Utilities.TryGetComponentInChildren<CableParentAttributes>(plug);
            for(int i=startingIndex; i<parentAttributes.cables.Count; i++) {
                CableChildAttributes childAttributes = Utilities.TryGetComponentInChildren<CableChildAttributes>(parentAttributes.cables[i].gameObject);
                if(parentAttributes.cables[i].gameObject.activeSelf) { 
                    indexAndDirections.Add(new IndexAndDirection(i-1, childAttributes.endingDirection)); 
                }
            }
            data.levelsSavePlugs[levelIndex].Add(new SavePlug(plugPosition, isPluggedIn, indexAndDirections));
        }
    }
    public void SaveDataLate(GameData data) {}

    private IEnumerator TestForLoadData() {
        yield return new WaitForSeconds(1.5f);
        if(!finishedWithAllTasks) {
            Debug.LogWarning("Level failed to load. trying again.");
            DataPersistenceManager.instance.LoadGame();
            StartCoroutine(TestForLoadData());
        }
    }



    new void Awake() {
        base.Awake();
        levelIndex = SceneManager.GetActiveScene().buildIndex - Constants.firstLevelBuidIndex;
    }

    new void Start() {
        base.Start();
        DebugC = DebugC.Get();
        gridsSkeleton = FindObjectOfType<GridsSkeleton>();
        Initialize();
    }

    void Update() {
        /*
        if(!allCableHandlersInitializationFinished) {
            CableParentAttributes[] cableParentAttributes = FindObjectsOfType<CableParentAttributes>();
            for(int i=0; i<cableParentAttributes.Length; i++) {
                if(!cableParentAttributes[i].finishedInitialization) {
                    return;
                }
            }
            allCableHandlersInitializationFinished = true;
        }
        */
    }

    private void Initialize() {
        ResetPlugs();
        //Log();
        StartCoroutine(base.SetMenuButton(false));
        StartCoroutine(base.SetLevelSelectorButton(true));
        StartCoroutine(base.SetTutorialHelpButton(true));
        initializationFinished = true;
        //StartCoroutine(TestForLoadData());
    }

    public void ResetPlugs() {
        RenewAllLevelPlugsList();
        SortAllLevelPlugs();
        RenewPlugSiblingIndices();
        MoveAllPlugsToInitialPositions();
    }

    public void AddPlugs() {
        RenewAllLevelPlugsList();
        SortAllLevelPlugs();
    }


    private IEnumerator EnableMenuButtons() {
        yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonsAttributes buttonAttribute in buttonsAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterMenu) {
                buttonAttribute.button.gameObject.SetActive(true);
            }
        }
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
            Directions startingDirection = Utilities.TryGetComponentInChildren<CableParentAttributes>(plugAttribute.gameObject).startingDirection;
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
        //Adds all the sorted plugs into one list as well
        sortedPlugs = new List<PlugAttributes>();
        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            foreach(PlugAttributes plugAttribute in levelPlug.plugAttributes) {
                sortedPlugs.Add(plugAttribute);
            }
        }
    }

    private void MoveAllPlugsToInitialPositions() {
        Vector2[,] skeletonGrid = gridsSkeleton.jointsSkeletonGrid;

        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            Directions direction = levelPlug.startingDirection;
            Index2D index;
            switch(direction) {
                case Directions.Up:
                    index = new Index2D((int)Constants.startingPlugOffset.x-1, (int)Constants.startingPlugOffset.y-1);
                    //goes to the right
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(skeletonGrid[index.x, index.y].x + Constants.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          skeletonGrid[index.x, index.y].y - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        Debug.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x, index.y + (int)plugAttributes.plugSize.y);
                    }
                    break;
                case Directions.Down:
                    index = new Index2D(skeletonGrid.GetLength(0)-(int)Constants.startingPlugOffset.x, (int)Constants.startingPlugOffset.y-1);
                    //goes to the right
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(skeletonGrid[index.x, index.y].x + Constants.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          skeletonGrid[index.x, index.y].y + (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        Debug.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x, index.y + (int)plugAttributes.plugSize.y);
                    }
                    break;
                case Directions.Left:
                    index = new Index2D((int)Constants.startingPlugOffset.x, (int)Constants.startingPlugOffset.y-1);
                    //goes down
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(skeletonGrid[index.x, index.y].x,
                                                          skeletonGrid[index.x, index.y].y - Constants.jointDistance*((plugAttributes.plugSize.x-1)/2) - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,//*(i+1),
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        Debug.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x + (int)plugAttributes.plugSize.x, index.y);
                    }
                    break;
                case Directions.Right:
                    index = new Index2D((int)Constants.startingPlugOffset.x, skeletonGrid.GetLength(1)-(int)Constants.startingPlugOffset.y);
                    //goes down
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(skeletonGrid[index.x, index.y].x - Constants.jointDistance*Constants.startingPlugOffsetRightSideAdd,
                                                          skeletonGrid[index.x, index.y].y - Constants.jointDistance*((plugAttributes.plugSize.x-1)/2) - (Constants.startingPlugOffset.x-(int)Constants.startingPlugOffset.x)*Constants.jointDistance,//*(i+1),
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        Debug.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        index = new Index2D(index.x + (int)plugAttributes.plugSize.x, index.y);
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
