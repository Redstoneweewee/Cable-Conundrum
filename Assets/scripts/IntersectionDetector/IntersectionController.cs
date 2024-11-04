using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntersectionController : MonoBehaviour {
    IntersectionData D;

    void Start() {
        D = Utilities.TryGetComponent<IntersectionData>(gameObject);
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
        D.allObstaclesGrid = new bool[D.jointsData.jointsGrid.GetLength(0), D.jointsData.jointsGrid.GetLength(1)];
        ObstacleAttributes[] obstacleAttributes = FindObjectsOfType<ObstacleAttributes>();
        foreach(ObstacleAttributes obstacleAttribute in obstacleAttributes) {
            if(obstacleAttribute.obstacleType == ObstacleTypes.Plug) { continue; }
            if(obstacleAttribute.obstacleGrid == null) { Debug.LogWarning($"{obstacleAttribute.name}'s obstaclesGrid not defined."); continue; }

            for(int i=0; i<D.allObstaclesGrid.GetLength(0); i++) {
                for(int j=0; j<D.allObstaclesGrid.GetLength(1); j++) {
                    if(obstacleAttribute.obstacleGrid[i,j] == true) {
                        D.allObstaclesGrid[i,j] = true;
                    }
                }
            }
        }
        string text = "";
        for(int i=0; i<D.allObstaclesGrid.GetLength(0); i++) {
            text += "| ";
            for(int j=0; j<D.allObstaclesGrid.GetLength(1); j++) {
                if(D.allObstaclesGrid[i,j] == true) { text += "*  "; }
                else { text += "-  "; }
            }
            text += " |\n";
        }
        Debug.Log("allObstaclesGrid: \n"+text);
    }

    public void TestForCableIntersection() {
        ClearAllCableIntersections();
        for(int i=0; i<D.electricalStripData.allCablesGrid.GetLength(0); i++) {
            for(int j=0; j<D.electricalStripData.allCablesGrid.GetLength(1); j++) {
                //if it is greater or equal to 2, there are intersections at that joint position
                if(D.electricalStripData.plugsGrid[i,j] > 0) {
                    DetermineTypeOfIntersection(i, j, D.electricalStripData.plugsGrid[i,j]);
                }
                else if(D.electricalStripData.allCablesGrid[i,j] >= 2 || D.allObstaclesGrid[i,j] == true) {
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
                Image cableImage = Utilities.TryGetComponentInChildren<Image>(cable.gameObject);
                if(!plug.isObstacle) { cableImage.color = new Color(1, 1, 1, cableImage.color.a); }
                else { cableImage.color = new Color(Constants.obstacleCableColor.r, Constants.obstacleCableColor.g, Constants.obstacleCableColor.b, cableImage.color.a); }
            }
        }
        D.hasIntersection = false;
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
                Image cableImage1 = Utilities.TryGetComponentInChildren<Image>(cableParentAttributes.cables[index1].gameObject);
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
                
                if(Utilities.TryGetComponent<CableChildAttributes>(cableParentAttributes.cables[index1].gameObject).isInitialCable) { continue; }
                int index2 = cableParentAttributes.cableGrid[i,j].numbers[0]-1;
                if(index2 < 0) { continue; }
                Image cableImage2 = Utilities.TryGetComponentInChildren<Image>(cableParentAttributes.cables[index2].gameObject);
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
                D.hasIntersection = true;
            }
        }
    }
}
