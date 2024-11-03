using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSwitch : MonoBehaviour, IPointerClickHandler {
    public DebugC DebugC {set; get;}
    [SerializeField] private GameObject offVisual;
    [SerializeField] private GameObject onVisual;
    private ElectricalStripController electricalStripController;
    private IntersectionDetector intersectionDetector;
    // Start is called before the first frame update
    void Start() {
        DebugC = DebugC.Get();
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        intersectionDetector = FindObjectOfType<IntersectionDetector>();
    }

    // Update is called once per frame
    void Update() {

    }
    public void OnPointerClick(PointerEventData eventData) {
        LevelFailureTypes levelFailureTypes = TestForLevelSuccess();
        if     (levelFailureTypes == LevelFailureTypes.Plugs)     { Debug.Log("Not all plugs are plugged in!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Obstacles) { Debug.Log("Some cables are in obstacles!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Cables)    { Debug.Log("Some cables are overlapping!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.None)      { Debug.Log("You win!"); Win(); }
        else   { Debug.LogError("Undefined level failure type."); }
        DebugC.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    //conditions:
    //all plugs must be plugged in
    //no cables are overlapping
    private LevelFailureTypes TestForLevelSuccess() {
        Plug[] allPlugs = FindObjectsOfType<Plug>();
        foreach(Plug plug in allPlugs) {
            if(plug.isObstacle) { continue; }
            if(!plug.isPluggedIn) { return LevelFailureTypes.Plugs; }
        }

        if(intersectionDetector.HasIntersection) {
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
        offVisual.SetActive(false);
        onVisual.SetActive(true);
    }

    private void DidNotWin() {
        offVisual.SetActive(true);
        onVisual.SetActive(false);
    }
}
