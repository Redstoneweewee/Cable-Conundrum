using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class IntersectionController : Singleton<IntersectionController> {
    IntersectionData D;

    public override IEnumerator Initialize() {
        D = IntersectionData.Instance;
        TestForCableIntersection();
        //StartCoroutine(InitialWaitUntilUpdate());
        yield return null;
    }

    public void TestForCableIntersection() {
        ClearAllCableIntersections();
        for(int i=0; i<GridsData.Instance.allCablesGrid.GetLength(0); i++) {
            for(int j=0; j<GridsData.Instance.allCablesGrid.GetLength(1); j++) {
                //if it is greater or equal to 2, there are intersections at that joint position
                if(GridsData.Instance.plugsGrid[i,j] > 0) {
                    DetermineTypeOfIntersection(i, j, GridsData.Instance.plugsGrid[i,j]);
                }
                else if(GridsData.Instance.allCablesGrid[i,j] >= 2 || GridsData.Instance.allObstaclesGrid[i,j] == true) {
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
