using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class IntersectionController : MonoBehaviour {
    IntersectionData D;

    void Awake() {
        D = Utilities.TryGetComponent<IntersectionData>(gameObject);
    }

    void Start() {
        StartCoroutine(InitialWaitUntilUpdate(0.02f));
    }

    private IEnumerator InitialWaitUntilUpdate(float time) {
        yield return new WaitForSeconds(time);
        //RenewAllObstaclesGrid();
        TestForCableIntersection();
    }
    /*
    public void RenewAllObstaclesGrid() {
        gridsData.allObstaclesGrid = new bool[D.jointsData.jointsGrid.GetLength(0), D.jointsData.jointsGrid.GetLength(1)];
        ObstacleAttributes[] obstacleAttributes = FindObjectsByType<ObstacleAttributes>();
        foreach(ObstacleAttributes obstacleAttribute in obstacleAttributes) {
            if(obstacleAttribute.obstacleType == ObstacleTypes.Plug) { continue; }
            if(obstacleAttribute.obstacleGrid == null) { Debug.LogWarning($"{obstacleAttribute.name}'s obstaclesGrid not defined."); continue; }

            for(int i=0; i<gridsData.allObstaclesGrid.GetLength(0); i++) {
                for(int j=0; j<gridsData.allObstaclesGrid.GetLength(1); j++) {
                    if(obstacleAttribute.obstacleGrid[i,j] == true) {
                        gridsData.allObstaclesGrid[i,j] = true;
                    }
                }
            }
        }
        string text = "";
        for(int i=0; i<gridsData.allObstaclesGrid.GetLength(0); i++) {
            text += "| ";
            for(int j=0; j<gridsData.allObstaclesGrid.GetLength(1); j++) {
                if(gridsData.allObstaclesGrid[i,j] == true) { text += "*  "; }
                else { text += "-  "; }
            }
            text += " |\n";
        }
        Debug.Log("allObstaclesGrid: \n"+text);
    }
    */
    public void TestForCableIntersection() {
        ClearAllCableIntersections();
        for(int i=0; i<D.gridsData.allCablesGrid.GetLength(0); i++) {
            for(int j=0; j<D.gridsData.allCablesGrid.GetLength(1); j++) {
                //if it is greater or equal to 2, there are intersections at that joint position
                if(D.gridsData.plugsGrid[i,j] > 0) {
                    DetermineTypeOfIntersection(i, j, D.gridsData.plugsGrid[i,j]);
                }
                else if(D.gridsData.allCablesGrid[i,j] >= 2 || D.gridsData.allObstaclesGrid[i,j] == true) {
                    DetermineTypeOfIntersection(i, j, 0);
                }
            }
        }
    }

    public void ClearAllCableIntersections() {
        PlugAttributes[] allPlugAttributes = FindObjectsByType<PlugAttributes>(FindObjectsSortMode.None);
        foreach(PlugAttributes plugAttribute in allPlugAttributes) {
            ClearCableIntersections(plugAttribute);
        }
        D.hasIntersection = false;
    }

    public void ClearCableIntersections(PlugAttributes plugAttribute) {
        CableParentAttributes cableParentAttributes = plugAttribute.cableParentAttributes;
        int count = 1;
        foreach(Transform cable in cableParentAttributes.cables) {
            Image cableImage = Utilities.TryGetComponentInChildren<Image>(cable.gameObject);
            if(!plugAttribute.isObstacle) { 
                cableImage.color = new Color(1, 1, 1, cableImage.color.a); 
                Debug.Log($"changed color of cable {count}");
            }
            else { cableImage.color = new Color(Constants.obstacleCableColor.r, Constants.obstacleCableColor.g, Constants.obstacleCableColor.b, cableImage.color.a); }
            count++;
        }
    }

    private void DetermineTypeOfIntersection(int i, int j, int plugId) {
        PlugAttributes[] allPlugAttributes = FindObjectsByType<PlugAttributes>(FindObjectsSortMode.None);
        //CableGridAttributes[] allCablesAtPosition = new CableGridAttributes[cableGenerations.Length];
        //populates allCablesAtPosition to find out how the cables are overlapping
        foreach(PlugAttributes plugAttribute in allPlugAttributes) {
            CableParentAttributes cableParentAttributes = plugAttribute.cableParentAttributes;
            if(!plugAttribute.isPluggedIn) { continue; }
            if(cableParentAttributes.cableGrid == null) { continue; }
            if(!cableParentAttributes.cableGrid[i,j].hasCable) { continue; }
            if(plugId != 0 && plugAttribute.id == plugId) { continue; }
            foreach(int index1 in cableParentAttributes.cableGrid[i,j].numbers) {
                Image cableImage1 = Utilities.TryGetComponentInChildren<Image>(cableParentAttributes.cables[index1].gameObject);
                if(!plugAttribute.isObstacle) {
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
                
                //if(Utilities.TryGetComponent<CableChildAttributes>(cableParentAttributes.cables[index1].gameObject).isInitialCable) { continue; }
                int index2 = cableParentAttributes.cableGrid[i,j].numbers[0]-1;
                if(index2 < 0) { continue; }
                Image cableImage2 = Utilities.TryGetComponentInChildren<Image>(cableParentAttributes.cables[index2].gameObject);
                if(!plugAttribute.isObstacle) {
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
