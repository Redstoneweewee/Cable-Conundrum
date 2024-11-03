using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntersectionDetector : MonoBehaviour {
    private bool hasIntersection = false;
    ElectricalStripController electricalStripController;
    JointsController jointsController;
    private bool[,] allObstaclesGrid;

    public bool    HasIntersection  {get{return hasIntersection;}  set{hasIntersection  = value;}}
    public bool[,] AllObstaclesGrid {get{return allObstaclesGrid;} set{allObstaclesGrid = value;}}

    // Start is called before the first frame update
    void Start() {
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        jointsController = FindObjectOfType<JointsController>();
        StartCoroutine(InitialWaitUntilUpdate(0.02f));
    }

    // Update is called once per frame
    void Update() {
        //StartCoroutine(InitialWaitUntilUpdate(0.02f));
    }

    private IEnumerator InitialWaitUntilUpdate(float time) {
        yield return new WaitForSeconds(time);
        RenewAllObstaclesGrid();
        TestForCableIntersection();
    }

    public void RenewAllObstaclesGrid() {
        allObstaclesGrid = new bool[jointsController.JointsGrid.GetLength(0), jointsController.JointsGrid.GetLength(1)];
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach(Obstacle obstacle in obstacles) {
            if(obstacle.ObstacleType == ObstacleTypes.Plug) { continue; }
            if(obstacle.ObstacleGrid == null) { Debug.LogWarning($"{obstacle.name}'s obstaclesGrid not defined."); continue; }

            for(int i=0; i<allObstaclesGrid.GetLength(0); i++) {
                for(int j=0; j<allObstaclesGrid.GetLength(1); j++) {
                    if(obstacle.ObstacleGrid[i,j] == true) {
                        allObstaclesGrid[i,j] = true;
                    }
                }
            }
        }
        string text = "";
        for(int i=0; i<allObstaclesGrid.GetLength(0); i++) {
            text += "| ";
            for(int j=0; j<allObstaclesGrid.GetLength(1); j++) {
                if(allObstaclesGrid[i,j] == true) { text += "*  "; }
                else { text += "-  "; }
            }
            text += " |\n";
        }
        Debug.Log("allObstaclesGrid: \n"+text);
    }

    public void TestForCableIntersection() {
        ClearAllCableIntersections();
        for(int i=0; i<electricalStripController.AllCablesGrid.GetLength(0); i++) {
            for(int j=0; j<electricalStripController.AllCablesGrid.GetLength(1); j++) {
                //if it is greater or equal to 2, there are intersections at that joint position
                if(electricalStripController.PlugsGrid[i,j] > 0) {
                    DetermineTypeOfIntersection(i, j, electricalStripController.PlugsGrid[i,j]);
                }
                else if(electricalStripController.AllCablesGrid[i,j] >= 2 || allObstaclesGrid[i,j] == true) {
                    DetermineTypeOfIntersection(i, j, 0);
                }
            }
        }
    }

    public void ClearAllCableIntersections() {
        Plug[] plugs = FindObjectsOfType<Plug>();
        foreach(Plug plug in plugs) {
            CableParentAttributes cableParentAttributes = plug.cableParentAttributes;
            foreach(Transform cable in cableParentAttributes.cables) {
                Image cableImage = cable.GetComponentInChildren<Image>();
                if(!plug.isObstacle) { cableImage.color = new Color(1, 1, 1, cableImage.color.a); }
                else { cableImage.color = new Color(Constants.obstacleCableColor.r, Constants.obstacleCableColor.g, Constants.obstacleCableColor.b, cableImage.color.a); }
            }
        }
        hasIntersection = false;
    }

    private void DetermineTypeOfIntersection(int i, int j, int plugId) {
        Plug[] plugs = FindObjectsOfType<Plug>();
        //CableGridAttributes[] allCablesAtPosition = new CableGridAttributes[cableGenerations.Length];
        //populates allCablesAtPosition to find out how the cables are overlapping
        foreach(Plug plug in plugs) {
            CableParentAttributes cableParentAttributes = plug.cableParentAttributes;
            if(cableParentAttributes.cableGrid == null) { continue; }
            if(!cableParentAttributes.cableGrid[i,j].hasCable) { continue; }
            if(plugId != 0 && plug.id == plugId) { continue; }
            foreach(int index1 in cableParentAttributes.cableGrid[i,j].numbers) {
                Image cableImage1 = cableParentAttributes.cables[index1].GetComponentInChildren<Image>();
                if(!plug.isObstacle) {
                    cableImage1.color = new Color(Constants.cableIntersectionColor.r,
                                                  Constants.cableIntersectionColor.g,
                                                  Constants.cableIntersectionColor.b,
                                                  cableImage1.color.a);
                }
                else {
                    cableImage1.color = new Color(Constants.obstacleCableIntersectionColor.r,
                                                  Constants.obstacleCableIntersectionColor.g,
                                                  Constants.obstacleCableIntersectionColor.b,
                                                  cableImage1.color.a);
                }
                
                if(cableParentAttributes.cables[index1].GetComponent<CableChildAttributes>().isInitialCable) { continue; }
                int index2 = cableParentAttributes.cableGrid[i,j].numbers[0]-1;
                if(index2 < 0) { continue; }
                Image cableImage2 = cableParentAttributes.cables[index2].GetComponentInChildren<Image>();
                if(!plug.isObstacle) {
                    cableImage2.color = new Color(Constants.cableIntersectionColor.r,
                                                  Constants.cableIntersectionColor.g,
                                                  Constants.cableIntersectionColor.b,
                                                  cableImage2.color.a);
                }
                else {
                    cableImage2.color = new Color(Constants.obstacleCableIntersectionColor.r,
                                                  Constants.obstacleCableIntersectionColor.g,
                                                  Constants.obstacleCableIntersectionColor.b,
                                                  cableImage2.color.a);
                }
                hasIntersection = true;
            }

        }
    }
}
