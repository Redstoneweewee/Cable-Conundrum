using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//Note: Place this script next to CableParentAttributes

//[ExecuteInEditMode]
public class InitialCableGenerator : MonoBehaviour {
    [Header("Place this script next to CableParentAttributes.\n"+
            "**Make sure to remove this script before entering play mode.**")]
    [SerializeField] CableParentAttributes cableParentAttributes;
    [SerializeField] Directions generateDirection;
    [SerializeField] bool generateCable = false;

    void Update() {
        if(cableParentAttributes != null && generateCable) { 
            int previousIndex = cableParentAttributes.initialCables.Count-1;
            GenerateCable(previousIndex, generateDirection);
            RenewRotationAndIntersectionCables();
            generateCable = false;
        }
    }

    //Generates a straight cable (and a rotation cable if necessary) based on the grid attribute of the previous joint that the player was hovering over.
    //Uses the previous cable as a reference.
    private void GenerateCable(int previousIndex, Directions newDirection) {
        Transform previousCable;
        CableChildAttributes previousAttributes;
        previousCable = cableParentAttributes.initialCables[previousIndex].transform;
        previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        
        //going straight
        if(previousAttributes.endingDirection == newDirection) {
            GenerateStraightCable(previousIndex+1);
        }
        //turning
        else if(previousAttributes.endingDirection != newDirection) {
            GenerateRotationCable(previousIndex+1, previousAttributes.endingDirection, newDirection);
        }
        DebugC.Instance?.LogList("initialCables: ", cableParentAttributes.initialCables);
    }


    private void GenerateStraightCable(int index) {
        Transform  previousCable = cableParentAttributes.initialCables[index-1].transform;
        CableChildAttributes previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        Directions previousEndingDirection = previousAttributes.endingDirection;
        ShadowDirections shadowDirection = Utilities.GetShadowDirectionForStraightCables(previousAttributes.shadowDirection, previousAttributes.startingDirection, previousAttributes.isRotationCable);
        
        GameObject cablePrefab = Utilities.GetStraightCablePrefab(CablePrefabs.Instance, shadowDirection, previousEndingDirection);
        CableChildAttributes prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(cablePrefab);
        Vector3    deltaPosition;
        if(!previousAttributes.isRotationCable) { deltaPosition = LevelResizeGlobal.Instance.jointDistance*prefabAttributes.directionMultiple; }
        else                                    { deltaPosition = Vector3.zero; }
        
        cableParentAttributes.initialCables.Add(Instantiate(cablePrefab, transform));
        cableParentAttributes.initialCables[index].transform.position = cableParentAttributes.initialCables[index-1].transform.position + deltaPosition;
        
        cableParentAttributes.initialCables[index].name = "InitialCable"+index;
        Utilities.TryGetComponent<CableChildAttributes>(cableParentAttributes.initialCables[index]).isInitialCable = true;
        Utilities.ModifyCableColorsToObstacle(cableParentAttributes.initialCables[index]);
    }

    private void GenerateRotationCable(int index, Directions startingDirection, Directions endingDirection) {
        Transform previousCable = cableParentAttributes.initialCables[index-1].transform;
        CableChildAttributes previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        ShadowDirections shadowDirection = Utilities.GetShadowDirectionForRotationCables(previousAttributes.shadowDirection, endingDirection);
        GameObject rotationCablePrefab = Utilities.GetRotationCablePrefab(CablePrefabs.Instance, shadowDirection, startingDirection, endingDirection);
        Vector3 deltaPosition = LevelResizeGlobal.Instance.jointDistance*previousAttributes.directionMultiple;
        Vector2 placePosition = previousCable.position + deltaPosition;

        Transform newCable = Instantiate(rotationCablePrefab, transform).transform;
        newCable.position = placePosition;
        cableParentAttributes.initialCables.Add(newCable.gameObject);
    
        cableParentAttributes.initialCables[index].name = "InitialRotationCable"+index;
        Utilities.TryGetComponent<CableChildAttributes>(cableParentAttributes.initialCables[index]).isInitialCable = true;
        Utilities.ModifyCableColorsToObstacle(cableParentAttributes.initialCables[index]);
    }

    
    private void RenewRotationAndIntersectionCables() {
        for(int i=0; i<cableParentAttributes.initialCables.Count; i++) {
            cableParentAttributes.initialCables[i].transform.SetSiblingIndex(i);
        }
        int index = 0;
        
        while(index < cableParentAttributes.initialCables.Count) {
            int nextRotationCableIndex = index;
            //finds the next rotation cable and sets nextIndex to its index
            for(int i=index; i<cableParentAttributes.initialCables.Count; i++) {
                
                if((Utilities.TryGetComponent<CableChildAttributes>(cableParentAttributes.initialCables[i]).isRotationCable || 
                    Utilities.TryGetComponent<CableChildAttributes>(cableParentAttributes.initialCables[i]).isIntersectionCable ) && 
                    cableParentAttributes.initialCables[i].gameObject.activeSelf) { 
                    nextRotationCableIndex = i; 
                    cableParentAttributes.initialCables[i].transform.SetSiblingIndex(i+1);
                    break; 
                }
                if(i == cableParentAttributes.initialCables.Count - 1) { return; }
            }

            //sets index to one after the rotation cable to search for next rotation cable
            index = nextRotationCableIndex + 1;
        }
    }
}
