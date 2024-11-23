using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSwitchController : MonoBehaviour, IPointerClickHandler, IDataPersistence {
    private PowerSwitchData D;

    public IEnumerator LoadData(GameData data) {
        yield return null;
        if(data.levelCompletion[D.levelInitializerGlobal.levelIndex]) {
            Win();
        }
    }

    public void SaveData(GameData data) {}
    public void SaveDataLate(GameData data) {}


    void Awake() {
        D = Utilities.TryGetComponent<PowerSwitchData>(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        LevelFailureTypes levelFailureTypes = TestForLevelSuccess();
        if     (levelFailureTypes == LevelFailureTypes.Plugs)     { Debug.Log("Not all plugs are plugged in!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Obstacles) { Debug.Log("Some cables are in obstacles!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Cables)    { Debug.Log("Some cables are overlapping!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.None)      { Debug.Log("You win!"); Win(); }
        else   { Debug.LogError("Undefined level failure type."); }
        D.debugC.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    //conditions:
    //all plugs must be plugged in
    //no cables are overlapping
    private LevelFailureTypes TestForLevelSuccess() {
        PlugAttributes[] allPlugAttributes = FindObjectsOfType<PlugAttributes>();
        foreach(PlugAttributes plugAttributes in allPlugAttributes) {
            if(plugAttributes.isObstacle) { continue; }
            if(!plugAttributes.isPluggedIn) { return LevelFailureTypes.Plugs; }
        }

        if(D.intersectionData.hasIntersection) {
            return LevelFailureTypes.Cables;
        }
        return LevelFailureTypes.None;
        /*
        JointsController  jointsController    = FindObjectOfType<JointsController>();
        Plug[]            allPlugs            = FindObjectsOfType<Plug>();

        
        foreach(Plug plug in allPlugs) {
            if(plug.IsObstacle) { continue; }
            if(!plug.isPluggedIn) { return LevelFailureTypes.Plugs; }
        }

        bool[,] allObstaclesGrid = intersectionDetector.AllObstaclesGrid;
        int[,] allCablesGrid = electricalStripController.AllCablesGrid;
        int[,] plugsGrid = electricalStripController.PlugsGrid;
        foreach(Plug plug in allPlugs) {
            if(plug.IsObstacle) { continue; }
            CableGeneration cableGeneration = plug.GetComponentInChildren<CableGeneration>();
            for(int i=0; i<cableGeneration.CableGrid.GetLength(0); i++) {
                for(int j=0; j<cableGeneration.CableGrid.GetLength(1); j++) {
                    if(allCablesGrid[i,j] >= 1 && allObstaclesGrid[i,j] == true) { return LevelFailureTypes.Obstacles; }
                    //if(plugsGrid[i,j] != 0 && allCablesGrid[i,j] >= 1 && plugsGrid[i,j] != plug.Id) { 
                    //    Debug.Log($"intersection at: {i}, {j}, plugsGrid: {plugsGrid[i,j]}, allCablesGrid: {allCablesGrid[i,j]}, plugId: {plug.Id}");
                    //    return LevelFailureTypes.Cables; 
                    //    
                    //}
                    if(allCablesGrid[i,j] >= 2) { return LevelFailureTypes.Cables; }
                }
            }
        }
        //DebugC.LogArray2DAlways("allCableGrids: ", allCableGrids);
        return LevelFailureTypes.None;
        */
    }

    private void Win() {
        D.offVisual.SetActive(false);
        D.onVisual.SetActive(true);
        D.winningControllerGlobal.OnWin();
    }

    private void DidNotWin() {
        D.offVisual.SetActive(true);
        D.onVisual.SetActive(false);
    }
}
