using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CableHandler : MonoBehaviour {
    private GridsController gridsController;
    private LevelInitializerGlobal levelInitializerGlobal;
    private CableParentAttributes A;

    void Awake() {
        gridsController = FindObjectOfType<GridsController>();
        levelInitializerGlobal = FindObjectOfType<LevelInitializerGlobal>();
        A = Utilities.TryGetComponent<CableParentAttributes>(gameObject);
    }

    void Start() {
        Initialize();
        RenewRotationAndIntersectionCables();
    }

    // Update is called once per frame
    void Update() {
        if(transform.position != A.cachedPosition || A.startingDirection != A.cachedStartingDirection || A.endingDirection != A.cachedEndingDirection) {
            GenerateEndingCables(A.lastRotationCableIndex+1);
            A.cachedPosition = transform.position;
            A.cachedStartingDirection = A.startingDirection;
            A.cachedEndingDirection = A.endingDirection;
        }
    }
    void LateUpdate() {
        if(A.renewSiblingCableIndices) {
            RenewRotationAndIntersectionCables();
            A.renewSiblingCableIndices = false;
        }
    }

    private void Initialize() {
        int endingCablesIndex = A.initialCables.Count;
        InitializeInitialCables();
        if(A.startingDirection != A.endingDirection) { TryRenewRotationCable(A.initialCables.Count, A.startingDirection, A.endingDirection); endingCablesIndex++; }
        GenerateEndingCables(endingCablesIndex);
        A.finishedInitialization = true;
    }

    public void ResetCableGrid() {
        if(A.cableGrid == null) { return; }
        for(int i=0; i<A.cableGrid.GetLength(0); i++) {
            for(int j=0; j<A.cableGrid.GetLength(1); j++) { 
                A.cableGrid[i,j].hasCable = false; 
                A.cableGrid[i,j].numbers.Clear(); 
            }
        }
    }
    public void InitializeCableGrid() {
        Vector2[,] skeletonGrid = A.gridsSkeleton.jointsSkeletonGrid;
        A.cableGrid = new CablesGridAttributes[skeletonGrid.GetLength(0), skeletonGrid.GetLength(1)];
        for(int i=0; i<A.cableGrid.GetLength(0); i++) {
            for(int j=0; j<A.cableGrid.GetLength(1); j++) { A.cableGrid[i,j] = new CablesGridAttributes(); }
        }
        float   subJointLength  = Constants.jointDistance/2;
        for(int i=0; i<A.cables.Count; i++) {
            if(A.cables[i].gameObject.activeSelf == false) { continue; }
            Vector2 distanceFromTopLeftJoint = new Vector2(A.cables[i].position.x - skeletonGrid[0,0].x, skeletonGrid[0,0].y - A.cables[i].position.y);
            Index2D gridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2);
            if(gridIndex.x >= A.cableGrid.GetLength(0) || gridIndex.x < 0 || gridIndex.y >= A.cableGrid.GetLength(1) || gridIndex.y < 0) { continue; }
            A.cableGrid[gridIndex.x,gridIndex.y].ChangeAttributes(true, i);
        }
        /*
        string text = "";
        for(int i=0; i<A.cableGrid.GetLength(0); i++) {
            text += "| ";
            for(int j=0; j<A.cableGrid.GetLength(1); j++) {
                if(A.cableGrid[i,j].hasCable == true) { text += "*  "; }
                //if(cableGrid[i,j].hasCable == true) { text += cableGrid[i,j].number + " "; }
                else { text += "-  "; }
            }
            text += " |\n";
        }
        A.debugC.Log("cableGrid: \n"+text);
        */

    }
    public void InitializeCachedMouseGridIndex() {
        Vector2[,] skeletonGrid = A.gridsSkeleton.jointsSkeletonGrid;
        float   subJointLength  = Constants.jointDistance/2;
        Vector2 distanceFromTopLeftJoint = new Vector2(A.mouse.position.value.x - skeletonGrid[0,0].x, skeletonGrid[0,0].y - A.mouse.position.value.y);
        A.cachedMouseGridIndex = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        A.cachedMouseGridIndex = new Index2D(Math.Clamp(A.cachedMouseGridIndex.y, 0, skeletonGrid.GetLength(0)-1), Math.Clamp(A.cachedMouseGridIndex.x, 0, skeletonGrid.GetLength(1)-1));
    }

    public IEnumerator ModifyCablesOnInteract() {
        yield return new WaitForSeconds(0.01f);

        Vector2[,] skeletonGrid = A.gridsSkeleton.jointsSkeletonGrid;
        float   subJointLength  = Constants.jointDistance/2;
        Vector2 distanceFromTopLeftJoint = new Vector2(A.mouse.position.value.x - skeletonGrid[0,0].x, skeletonGrid[0,0].y - A.mouse.position.value.y);
        Index2D mouseGridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        mouseGridIndex          = new Index2D(Math.Clamp(mouseGridIndex.y, 0, skeletonGrid.GetLength(0)-1), Math.Clamp(mouseGridIndex.x, 0, skeletonGrid.GetLength(1)-1));

        if(A.cachedMouseGridIndex != mouseGridIndex && !A.cableGrid[mouseGridIndex.x, mouseGridIndex.y].hasCable) {
            InitializeCableGrid();
            if(A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers != null && A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers.Count > 0) {
                int cachedIndex = A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers[0];
                Debug.Log("cachedIndex: "+cachedIndex);
                if(cachedIndex < A.initialCables.Count) { Debug.Log("trying to change an initial cable. Stopped loop."); yield break; }
                Index2D deltaGridIndex = new Index2D(mouseGridIndex.x - A.cachedMouseGridIndex.x, mouseGridIndex.y - A.cachedMouseGridIndex.y);
                int previousIndex = cachedIndex - 1;
                if(previousIndex < 0) { previousIndex = 0; }
                if(deltaGridIndex.x == -1)      { A.endingDirection = Directions.Up; }
                else if(deltaGridIndex.x == 1)  { A.endingDirection = Directions.Down; }
                else if(deltaGridIndex.y == -1) { A.endingDirection = Directions.Left; }
                else if(deltaGridIndex.y == 1)  { A.endingDirection = Directions.Right; }
                Directions startDirection = Utilities.TryGetComponent<CableChildAttributes>(A.cables[previousIndex].gameObject).endingDirection;
                Index2D gridIntersectionIndex = TestForIntersections(A.cachedMouseGridIndex, A.endingDirection);
                if(gridIntersectionIndex != new Index2D(-1, -1)) {
                    A.debugC.Log($"tried to create a loop. stopping cable generation. starting index: ({mouseGridIndex.x}, {mouseGridIndex.y}), direction: {A.endingDirection}, intersectionindex: ({gridIntersectionIndex.x}, {gridIntersectionIndex.y})");
                    int resetToIndex = A.cableGrid[gridIntersectionIndex.x, gridIntersectionIndex.y].numbers[0];
                    if(resetToIndex < A.initialCables.Count) { resetToIndex = A.initialCables.Count; }
                    Transform resetToCable = A.cables[resetToIndex];
                    A.endingDirection = Utilities.TryGetComponent<CableChildAttributes>(resetToCable.gameObject).endingDirection;
                    GenerateEndingCables(resetToIndex);
                    RenewRotationAndIntersectionCables();
                    A.cachedMouseGridIndex = mouseGridIndex;
                }
                else if(!DirectionsHaveError(startDirection, A.endingDirection)) {
                    if(deltaGridIndex.x == -1) { //moved up
                        A.debugC.Log("moved up");
                        TryGenerateCable(previousIndex, Directions.Up);
                    }
                    else if(deltaGridIndex.x == 1) { //moved down
                        A.debugC.Log("moved down");
                        TryGenerateCable(previousIndex, Directions.Down);
                    }
                    else if(deltaGridIndex.y == -1) { //moved left
                        A.debugC.Log("moved left");
                        TryGenerateCable(previousIndex, Directions.Left);
                    }
                    else if(deltaGridIndex.y == 1) { //moved right
                        A.debugC.Log("moved right");
                        TryGenerateCable(previousIndex, Directions.Right);
                    }
                    else {
                        Debug.LogWarning("Moved mouse too quickly");
                    }
                    A.cachedMouseGridIndex = mouseGridIndex;
                }
                else {
                    Debug.LogWarning($"Impossible directions. Starting: {startDirection}, Ending: {A.endingDirection}");
                    for(int i=A.cables.Count-1; i>=0; i--) {
                        if(A.cables[i].gameObject.activeSelf) { 
                            A.endingDirection = Utilities.TryGetComponent<CableChildAttributes>(A.cables[i].gameObject).endingDirection; 
                            break;
                        }
                    }
                    StopCoroutine(A.plugAttributes.modifyCableCoroutine);
                    A.plugAttributes.modifyCableCoroutine = null;
                    yield break;
                }
            }
            InitializeCableGrid();
            //A.electricalStripController.RenewAllCableGrids();
            gridsController.RenewAllCablesGrid();
            A.intersectionController.TestForCableIntersection();
        }
        //moving into an already defined cable
        else if(A.cachedMouseGridIndex != mouseGridIndex && A.cableGrid[mouseGridIndex.x, mouseGridIndex.y].hasCable) {

                if(A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers != null && A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers.Count > 0) {
                    int index = A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers[0];
                    if(index < A.initialCables.Count) { Debug.Log("trying to change an initial cable. Stopped loop.");yield break; }
                    Debug.Log("index: "+index);
                    Transform previousCable = A.cables[index-1];
                    A.endingDirection = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject).endingDirection;
                    GenerateEndingCables(index);
                }
                RenewRotationAndIntersectionCables();
                A.cachedMouseGridIndex = mouseGridIndex;
            InitializeCableGrid();
            //A.electricalStripController.RenewAllCableGrids();
            gridsController.RenewAllCablesGrid();
            A.intersectionController.TestForCableIntersection();
        }
        


        if(A.plugAttributes.isModifyingCables) {
            A.plugAttributes.modifyCableCoroutine = ModifyCablesOnInteract();
            StartCoroutine(A.plugAttributes.modifyCableCoroutine);
        }
        else { 
            if(A.plugAttributes.modifyCableCoroutine != null) { StopCoroutine(A.plugAttributes.modifyCableCoroutine); }
            A.plugAttributes.modifyCableCoroutine = null;
        }
    }

    private Index2D TestForIntersections(Index2D index2D, Directions direction) {
        int[,] plugsGrid = A.gridsData.plugsGrid;
        switch(direction) {
            case Directions.Up:
                for(int i=index2D.x-1; i>=0; i--) {
                    if(plugsGrid[i, index2D.y] == A.plugAttributes.id) { return index2D; }
                    if(A.cableGrid[i, index2D.y].hasCable) { return index2D; }
                }
                break;
            case Directions.Down:
                for(int i=index2D.x+1; i<A.cableGrid.GetLength(0); i++) {
                    if(plugsGrid[i, index2D.y] == A.plugAttributes.id) { return index2D; }
                    if(A.cableGrid[i, index2D.y].hasCable) { return index2D; }
                }
                break;
            case Directions.Left:
                for(int i=index2D.y-1; i>=0; i--) {
                    if(plugsGrid[index2D.x, i] == A.plugAttributes.id) { return index2D; }
                    if(A.cableGrid[index2D.x, i].hasCable) { return index2D; }
                }
                break;
            case Directions.Right:
                for(int i=index2D.y+1; i<A.cableGrid.GetLength(1); i++) {
                    if(plugsGrid[index2D.x, i] == A.plugAttributes.id) { return index2D; }
                    if(A.cableGrid[index2D.x, i].hasCable) { return index2D; }
                }
                break;
        }
        return new Index2D(-1, -1);
    }

    private bool DirectionsHaveError(Directions start, Directions end) {
        if     (start == Directions.Up    && end == Directions.Down ) { return true; }
        else if(start == Directions.Down  && end == Directions.Up   ) { return true; }
        else if(start == Directions.Left  && end == Directions.Right) { return true; }
        else if(start == Directions.Right && end == Directions.Left ) { return true; }
        return false;
    }


    private void InitializeInitialCables() {
        foreach(GameObject initialCable in A.initialCables) {
            A.cables.Add(initialCable.transform);
            A.debugC.Log($"{initialCable.name} added to cables list for {transform.name}");
        }
    }
    private void RenewInitialCable() {
        if(A.cables.Count > 0) {
            CableChildAttributes prefabAttributes;
            switch(A.startingDirection) {
                case Directions.Up:
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs.cablePrefabs[6]); break;
                case Directions.Down:
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs.cablePrefabs[7]); break;
                case Directions.Left:
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs.cablePrefabs[2]); break;
                case Directions.Right:
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs.cablePrefabs[3]); break;
                default:
                    Debug.LogError("RenewInitialCable function did not work correctly. None of the conditions were met.");
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs.cablePrefabs[3]); break;
            }

            Sprite  prefabSprite    = A.cablePrefabs.cableSprites[prefabAttributes.cableSpriteIndex];
            float   prefabZRotation = prefabAttributes.zRotation;
            Vector2 prefabPivot     = prefabAttributes.pivot;
            Utilities.ModifyCableValues(A.cables[0], prefabAttributes, false, 
                                        prefabZRotation, Constants.straightCableSize, prefabPivot, prefabSprite);
        }
        else {
            Transform newCable;
            switch(A.startingDirection) {
                case Directions.Up:
                    newCable = Instantiate(A.cablePrefabs.cablePrefabs[6], transform).transform;
                    A.cables.Add(newCable);
                    break;
                case Directions.Down:
                    newCable = Instantiate(A.cablePrefabs.cablePrefabs[7], transform).transform;
                    A.cables.Add(newCable);
                    break;
                case Directions.Left:
                    newCable = Instantiate(A.cablePrefabs.cablePrefabs[2], transform).transform;
                    A.cables.Add(newCable);
                    break;
                case Directions.Right:
                    newCable = Instantiate(A.cablePrefabs.cablePrefabs[3], transform).transform;
                    A.cables.Add(newCable);
                    break;
            }
        }
        A.cables[0].name = "InitialCable0";
        A.cables[0].gameObject.SetActive(true);
    }

    private void RenewRotationAndIntersectionCables() {
        for(int i=A.initialCables.Count; i<A.cables.Count; i++) {
            A.cables[i].SetSiblingIndex(i);
        }
        int index = A.initialCables.Count;
        A.lastRotationCableIndex = A.initialCables.Count;
        
        while(index < A.cables.Count) {
            int nextRotationCableIndex = index;
            //finds the next rotation cable and sets nextIndex to its index
            for(int i=index; i<A.cables.Count; i++) {
                
                if((Utilities.TryGetComponent<CableChildAttributes>(A.cables[i].gameObject).isRotationCable || Utilities.TryGetComponent<CableChildAttributes>(A.cables[i].gameObject).isIntersectionCable ) && A.cables[i].gameObject.activeSelf) { 
                    nextRotationCableIndex = i; 
                    A.lastRotationCableIndex = i;
                    A.cables[i].SetSiblingIndex(i+1);
                    break; 
                }
                if(i == A.cables.Count - 1) { return; }
            }

            //sets index to one after the rotation cable to search for next rotation cable
            index = nextRotationCableIndex + 1;
        }
    }



    //Used in loading data stored in hardware
    public void TryGenerateCableFromList(List<IndexAndDirection> indexAndDirections) {
        A.debugC.Log("Generating Cables from List:");
        for(int i=0; i<indexAndDirections.Count; i++){
            A.debugC.Log($"Generating cable at index {indexAndDirections[i].previousIndex+1}, direction: {indexAndDirections[i].endingDirection}");
            TryGenerateCable(indexAndDirections[i].previousIndex, indexAndDirections[i].endingDirection);
        }
        InitializeCableGrid();
        gridsController.RenewAllCablesGrid();
        A.intersectionController.TestForCableIntersection();
    }

    //Generates a straight cable (and a rotation cable if necessary) based on the grid attribute of the previous joint that the player was hovering over.
    //Uses the previous cable as a reference.
    private void TryGenerateCable(int previousIndex, Directions newDirection) {
        Transform previousCable;
        CableChildAttributes previousAttributes;
        if(!Utilities.TryGetComponent<CableChildAttributes>(A.cables[previousIndex].gameObject).isRotationCable) {
            previousCable = A.cables[previousIndex];
            previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        }
        else {
            previousIndex -= 1;
            previousCable = A.cables[previousIndex];
            previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        }
        //going straight
        if(previousAttributes.endingDirection == newDirection) {
            TryRenewStraightCable(previousIndex+1);
        }
        //turning
        else if(previousAttributes.endingDirection != newDirection) {
            TryRenewRotationCable(previousIndex+1, previousAttributes.endingDirection, newDirection);
            A.debugC.Log("renewed rotation cable, direction: "+newDirection);
        }
        GenerateEndingCables(previousIndex+2);
        A.renewSiblingCableIndices = true;
    }


    private void GenerateEndingCables(int index) {
        for(int i=index; i<A.cables.Count; i++) { A.cables[i].gameObject.SetActive(false); }
        Transform previousCable = A.cables[index-1];
        Vector2 currentPosition = previousCable.position;
        switch(A.endingDirection) {
            case Directions.Up:
                while(currentPosition.y < Screen.height) {
                    TryRenewStraightCable(index); 
                    currentPosition.y += Constants.jointDistance;
                    index++;
                }
                break;
            case Directions.Down:
                while(currentPosition.y > 0) {
                    TryRenewStraightCable(index);
                    currentPosition.y -= Constants.jointDistance;
                    index++;
                }
                break;
            case Directions.Left:
                while(currentPosition.x > 0) {
                    TryRenewStraightCable(index);
                    currentPosition.x -= Constants.jointDistance;
                    index++;
                }
                break;
            case Directions.Right:
                while(currentPosition.x < Screen.width) {
                    TryRenewStraightCable(index);
                    currentPosition.x += Constants.jointDistance;
                    index++;
                }
                break;
        }
        if((!A.plugAttributes.isObstacle && A.plugAttributes.isPluggedIn) || (A.plugAttributes.isObstacle && !A.plugAttributes.obstacleAttributes.temporarilyModifiable)) { Utilities.SetCableOpacity(gameObject, 1f); }
        else if((!A.plugAttributes.isObstacle && !A.plugAttributes.isPluggedIn) || (A.plugAttributes.isObstacle && A.plugAttributes.obstacleAttributes.temporarilyModifiable)) {  Utilities.SetCableOpacity(gameObject, Constants.cableOpacity); }
        for(int i=A.cables.Count-1; i>=0; i--) {
            if(A.cables[i].gameObject.activeSelf) { A.endingDirection = Utilities.TryGetComponent<CableChildAttributes>(A.cables[i].gameObject).endingDirection; break; }
        }
        if(A.plugAttributes.isObstacle) { 
            foreach(Transform cable in A.cables) {
                Utilities.ModifyCableColorsToObstacle(cable.gameObject); 
            }
        }
        A.renewSiblingCableIndices = true;
    }


    private void TryRenewStraightCable(int index) {
        Transform  previousCable = A.cables[index-1];
        CableChildAttributes previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        Directions previousEndingDirection = previousAttributes.endingDirection;
        ShadowDirections shadowDirection = Utilities.GetShadowDirectionForStraightCables(previousAttributes.shadowDirection, previousAttributes.startingDirection, previousAttributes.isRotationCable);
        
        GameObject cablePrefab = Utilities.GetStraightCablePrefab(A.cablePrefabs, shadowDirection, previousEndingDirection);
        CableChildAttributes prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(cablePrefab);
        Vector3    deltaPosition;
        if(!previousAttributes.isRotationCable) { A.debugC.Log("previous is not rotation node"); deltaPosition = Constants.jointDistance*prefabAttributes.directionMultiple; }
        else                                   { A.debugC.Log("previous is rotation node"); deltaPosition = Vector3.zero; }
        
        
        if(A.cables.Count > index) {
            A.cables[index].position = previousCable.position + deltaPosition;
            Sprite     prefabSprite = A.cablePrefabs.cableSprites[prefabAttributes.cableSpriteIndex];
            float      prefabZRotation = prefabAttributes.zRotation;
            Vector2    prefabPivot = prefabAttributes.pivot;
            Utilities.ModifyCableValues(A.cables[index], prefabAttributes, false, 
                                        prefabZRotation, Constants.straightCableSize, prefabPivot, prefabSprite);
            A.cables[index].gameObject.SetActive(true);
        }
        else {
            A.cables.Add(Instantiate(cablePrefab, transform).transform);
            A.cables[index].position = A.cables[index-1].position + deltaPosition;
        }
        A.cables[index].name = "Cable"+index;
    }

    private void TryRenewRotationCable(int index, Directions startingDirection, Directions endingDirection) {
        Transform previousCable = A.cables[index-1];
        CableChildAttributes previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        ShadowDirections shadowDirection = Utilities.GetShadowDirectionForRotationCables(previousAttributes.shadowDirection, endingDirection);
        GameObject rotationCablePrefab = Utilities.GetRotationCablePrefab(A.cablePrefabs, shadowDirection, startingDirection, endingDirection);
        Vector3 deltaPosition = Constants.jointDistance*previousAttributes.directionMultiple;
        Vector2 placePosition = previousCable.position + deltaPosition;
        if(A.cables.Count > index) {
            CableChildAttributes prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(rotationCablePrefab);
            Sprite     prefabSprite = A.cablePrefabs.cableSprites[prefabAttributes.cableSpriteIndex];
            float      prefabZRotation = prefabAttributes.zRotation;
            Vector2    prefabPivot = prefabAttributes.pivot;

            A.cables[index].position = placePosition;
            Utilities.ModifyCableValues(A.cables[index], prefabAttributes, true, 
                                        prefabZRotation, Constants.rotationCableSize, prefabPivot, prefabSprite);
        }
        else {
            Transform newCable = Instantiate(rotationCablePrefab, transform).transform;
            newCable.position = placePosition;
            A.cables.Add(newCable);
        }
        A.cables[index].name = "RotationCable"+index;
    }

}
