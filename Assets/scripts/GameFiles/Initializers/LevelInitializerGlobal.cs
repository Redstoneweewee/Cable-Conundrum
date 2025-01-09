using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelInitializerGlobal : InitializerBase<LevelInitializerGlobal>, IDataPersistence {
    public int levelIndex;
    [HideInInspector] private List<LevelPlugs> allLevelPlugs = new List<LevelPlugs>();
    [HideInInspector] private List<PlugAttributes> sortedPlugs = new List<PlugAttributes>();
    private bool initializationFinished = false;
    //public bool allCableHandlersInitializationFinished = false;

    public override IEnumerator Initialize() {
        yield return null;
    }


    // |----------------------------------------------------------------------------------|
    // |----------------------------------------------------------------------------------|
    // |----- Save and LoadData does NOT work with PlugSelector dragged in plugs!!!! -----|
    // |----------------------------------------------------------------------------------|
    // |----------------------------------------------------------------------------------|
    
    //TryGenerateCableFromList
    public IEnumerator LoadData(GameData data) {
        Initialize(data);
        yield return new WaitUntil(() => initializationFinished);
        //yield return new WaitUntil(() => allCableHandlersInitializationFinished);
        //Debug.Log(data.levelsSavePlugs[levelIndex]);
        //Debug.Log(data.levelsSavePlugs[levelIndex].Count);
        if(data.levelsSavePlugs[levelIndex] != null && data.levelsSavePlugs[levelIndex].Count != 0) {
            List<SavePlug> levelData = data.levelsSavePlugs[levelIndex];
            //Set all the necessary data for a plug
            for(int i=0; i<levelData.Count; i++) {
                if(levelData[i] == null) { continue; }
                if(sortedPlugs.Count <= i) { continue; }

                SavePlug savePlug = levelData[i];
                if(!savePlug.isPluggedIn) { continue; }

                GameObject plug = sortedPlugs[i].gameObject;
                DebugC.Instance?.Log($"Inheriting values - receiver: {plug.name}");
                //DebugC.Instance?.Log($"plugPosition changed: {plug.name} from ({plug.transform.position}) to ({savePlug.plugPosition})");
                Vector3 socketPosition = GridsSkeleton.Instance.socketsSkeletonGrid[savePlug.socketIndex.x,savePlug.socketIndex.y];
                plug.transform.position = socketPosition-((Vector3)Utilities.TryGetComponent<PlugAttributes>(plug).localSnapPositions[0]*LevelResizeGlobal.Instance.finalScale);
                DebugC.Instance?.Log($"isPluggedIn changed: {plug.name} from ({Utilities.TryGetComponent<PlugAttributes>(plug).isPluggedIn}) to ({savePlug.isPluggedIn})");
                Utilities.TryGetComponent<PlugAttributes>(plug).isPluggedIn = savePlug.isPluggedIn;

                if(savePlug.indexAndDirections == null) { continue; }
                CableHandler cableHandler = Utilities.TryGetComponentInChildren<CableHandler>(plug);
                cableHandler.TryGenerateCableFromList(savePlug.indexAndDirections);
            }
        }

        //Once all data is loaded, we are finished with all tasks
        base.FinishedWithAllTasks();
        //And renew level grids
        StartCoroutine(FindFirstObjectByType<GridsController>().RenewGrids());
        MoveAllPlugsToInitialPositions();
    }

    
    public void SaveData(GameData data) {
        //If the level has already been completed, do not save any new changes
        if(data.levelCompletion[levelIndex]) { return; }

        data.levelsSavePlugs[levelIndex].Clear();
        DebugC.Instance?.LogListAlways("level savePlug: ", data.levelsSavePlugs[levelIndex]);
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
            if(isPluggedIn) {
                Vector3 socketSnapPosition = plugPosition+(Vector3)(Utilities.TryGetComponent<PlugAttributes>(plug).localSnapPositions[0]*LevelResizeGlobal.Instance.finalScale);
                Index2D socketIndex = Utilities.CalculateSocketsGridIndex(socketSnapPosition);
                data.levelsSavePlugs[levelIndex].Add(new SavePlug(socketIndex, isPluggedIn, indexAndDirections));
                Debug.Log($"socketSnapPosition: {socketSnapPosition}, socketIndex: [{socketIndex.x}, {socketIndex.y}], isPluggedIn: {isPluggedIn}, indexAndDirections: {indexAndDirections}");
            }
            else {
                data.levelsSavePlugs[levelIndex].Add(new SavePlug(isPluggedIn));
            }
        }
    }
    public void SaveDataLate(GameData data) {}

    //private IEnumerator TestForLoadData() {
    //    yield return new WaitForSeconds(1.5f);
    //    if(!finishedWithAllTasks) {
    //        Debug.LogWarning("Level failed to load. trying again.");
    //        DataPersistenceManager.Instance.LoadGame();
    //        StartCoroutine(TestForLoadData());
    //    }
    //}



    public override void OnAwake() {
        levelIndex = SceneManager.GetActiveScene().buildIndex - Constants.firstLevelBuidIndex;
    }

    new void Start() {
        base.Start();
    }

    void Update() {
        /*
        if(!allCableHandlersInitializationFinished) {
            CableParentAttributes[] cableParentAttributes = FindObjectsByType<CableParentAttributes>();
            for(int i=0; i<cableParentAttributes.Length; i++) {
                if(!cableParentAttributes[i].finishedInitialization) {
                    return;
                }
            }
            allCableHandlersInitializationFinished = true;
        }
        */
    }

    private void Initialize(GameData data) {
        bool shouldMovePlugs = data.levelsSavePlugs[levelIndex] == null || data.levelsSavePlugs[levelIndex].Count == 0;
        ResetPlugs(shouldMovePlugs);
        //Log();
        StartCoroutine(base.SetMenuButton(false));
        StartCoroutine(base.SetLevelSelectorButton(true));
        StartCoroutine(base.SetTutorialHelpButton(true));
        initializationFinished = true;
        //StartCoroutine(TestForLoadData());
    }

    public void ResetPlugs(bool shouldMovePlugs) {
        RenewAllLevelPlugsList();
        SortAllLevelPlugs();
        RenewPlugSiblingIndices();
        if(shouldMovePlugs) {
            MoveAllPlugsToInitialPositions();
        }
    }

    public void AddPlugs() {

        //|-------------------------------------------------------|
        //|-------------------------------------------------------|
        //| Added plugs do not get saved to data persistence yet. |
        //|-------------------------------------------------------|
        //|-------------------------------------------------------|

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

        PlugAttributes[] plugAttributes = FindObjectsByType<PlugAttributes>(FindObjectsSortMode.None);
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
    //3. A greater plug priority (if width and height are the same)
    //Ex sort. [3x2, 3x1, 2x3, 2x2, 2x1, 1x1]
    private void SortAllLevelPlugs() {
        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            for(int i=0; i<levelPlug.plugAttributes.Count; i++) {
                PlugAttributes largestPlug = levelPlug.plugAttributes[i];
                int largestPlugIndex = i;
                for(int j=i+1; j<levelPlug.plugAttributes.Count; j++) {
                    if(levelPlug.plugAttributes[j].plugSize.x > largestPlug.plugSize.x ||
                       (levelPlug.plugAttributes[j].plugSize.x == largestPlug.plugSize.x && levelPlug.plugAttributes[j].plugSize.y > largestPlug.plugSize.y) ||
                       (levelPlug.plugAttributes[j].plugSize.x == largestPlug.plugSize.x && levelPlug.plugAttributes[j].plugSize.y == largestPlug.plugSize.y && largestPlug.plugPriority < levelPlug.plugAttributes[j].plugPriority)) {
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
        Vector2[,] skeletonGrid = GridsSkeleton.Instance.jointsSkeletonGrid;

        foreach(LevelPlugs levelPlug in allLevelPlugs) {
            Directions direction = levelPlug.startingDirection;
            Vector3 plugPosition;
            switch(direction) {
                case Directions.Up:
                    plugPosition = new Vector3(LevelResizeGlobal.Instance.startingPlugOffset.x, Screen.height-LevelResizeGlobal.Instance.startingPlugOffset.y);
                    //goes to the right
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(plugPosition.x + LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          plugPosition.y - LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.x-1)/2),
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        DebugC.Instance?.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        plugPosition = new Vector3(plugPosition.x + ((int)plugAttributes.plugSize.y)*LevelResizeGlobal.Instance.jointDistance, plugPosition.y);
                    }
                    break;
                case Directions.Down:
                    plugPosition = new Vector3(LevelResizeGlobal.Instance.startingPlugOffset.x, LevelResizeGlobal.Instance.startingPlugOffset.y);
                    //goes to the right
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(plugPosition.x + LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          plugPosition.y + LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.x-1)/2),
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        DebugC.Instance?.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        plugPosition = new Vector3(plugPosition.x + ((int)plugAttributes.plugSize.y)*LevelResizeGlobal.Instance.jointDistance, plugPosition.y);
                    }
                    break;
                case Directions.Left:
                    plugPosition = new Vector3(LevelResizeGlobal.Instance.startingPlugOffset.x, Screen.height-LevelResizeGlobal.Instance.startingPlugOffset.y*2);
                    //goes down
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(plugPosition.x + LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          plugPosition.y - LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.x-1)/2),
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        DebugC.Instance?.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        plugPosition = new Vector3(plugPosition.x, plugPosition.y - ((int)plugAttributes.plugSize.x)*LevelResizeGlobal.Instance.jointDistance);
                    }
                    break;
                case Directions.Right:
                    plugPosition = new Vector3(Screen.width-LevelResizeGlobal.Instance.startingPlugOffset.x, Screen.height-LevelResizeGlobal.Instance.startingPlugOffset.y*2);
                    //goes down
                    foreach(PlugAttributes plugAttributes in levelPlug.plugAttributes) {
                        if(plugAttributes.isPluggedIn) { continue; }
                        Vector3 newPosition = new Vector3(plugPosition.x - LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.y-1)/2),
                                                          plugPosition.y - LevelResizeGlobal.Instance.jointDistance*((plugAttributes.plugSize.x-1)/2),
                                                          0);
                        plugAttributes.transform.position = newPosition - (Vector3)plugAttributes.center;
                        DebugC.Instance?.Log($"Plug {plugAttributes.name} moved to: ({newPosition.x}, {newPosition.y}, {newPosition.z})");
                        plugPosition = new Vector3(plugPosition.x, plugPosition.y - ((int)plugAttributes.plugSize.x)*LevelResizeGlobal.Instance.jointDistance);
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
