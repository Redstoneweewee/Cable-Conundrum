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
            CableGeneration cableGeneration = plug.CableGeneration;
            foreach(Transform cable in cableGeneration.Cables) {
                Image cableImage = cable.GetComponentInChildren<Image>();
                if(!plug.IsObstacle) { cableImage.color = new Color(1, 1, 1, cableImage.color.a); }
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
            CableGeneration cableGeneration = plug.CableGeneration;
            if(cableGeneration.CableGrid == null) { continue; }
            //allCablesAtPosition[a] = cableGeneration.CableGrid[i,j];
            if(!cableGeneration.CableGrid[i,j].hasCable) { continue; }
            if(plugId != 0 && plug.Id == plugId) { continue; }
            foreach(int index1 in cableGeneration.CableGrid[i,j].numbers) {
                Image cableImage1 = cableGeneration.Cables[index1].GetComponentInChildren<Image>();
                if(!plug.IsObstacle) {
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
                
                if(cableGeneration.Cables[index1].GetComponent<CableAttributes>().IsInitialCable) { continue; }
                int index2 = cableGeneration.CableGrid[i,j].numbers[0]-1;
                if(index2 < 0) { continue; }
                Image cableImage2 = cableGeneration.Cables[index2].GetComponentInChildren<Image>();
                if(!plug.IsObstacle) {
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

        //there can only be 4 cases: 
        // 1. size of a rotation cable (square)
        // 2. size of a straight cable facing left-right
        // 3. size of a straight cable facing up-down
        // 4. a combination of 2 & 3; a plus sign
        //Vector2 overlapSize = Constants.rotationCableSize;
        //Directions direction = Directions.Up; //direction doesn't matter if the overlap is a square

        /*
        * Algorithm: 
        * compare each cable with every other cable after it (normal comparation algorithum)
        * Check off 3 booleans--one for each of the first 3 cases
        * if both of the last 2 cases are true, then make a "+" shape
        * otherwise, use brain
        * Note: only have to generate intersection warning sprite after the comparation algorithm
        */
        //foreach(CableGridAttributes cableAtPosition in allCablesAtPosition) {

        //}
    }
}
