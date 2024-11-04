using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CableHandler : MonoBehaviour {
    CableParentAttributes A;

    void Start() {
        A = Utilities.TryGetComponent<CableParentAttributes>(gameObject);
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

    private void Initialize() {
        int endingCablesIndex = A.initialCables.Length;
        InitializeInitialCables();
        if(A.startingDirection != A.endingDirection) { TryRenewRotationCable(A.initialCables.Length, A.startingDirection, A.endingDirection); endingCablesIndex++; }
        GenerateEndingCables(endingCablesIndex);
    }

    public void ResetCableGrid() {
        for(int i=0; i<A.cableGrid.GetLength(0); i++) {
            for(int j=0; j<A.cableGrid.GetLength(1); j++) { 
                A.cableGrid[i,j].hasCable = false; 
                A.cableGrid[i,j].numbers.Clear(); 
            }
        }
    }
    public void InitializeCableGrid() {
        if(A.cachedJointsGrid == null || A.cachedJointsGrid != A.jointsData.jointsGrid) {
            A.cableGrid = new CablesGridAttributes[A.jointsData.jointsGrid.GetLength(0), A.jointsData.jointsGrid.GetLength(1)];
            for(int i=0; i<A.cableGrid.GetLength(0); i++) {
                for(int j=0; j<A.cableGrid.GetLength(1); j++) { A.cableGrid[i,j] = new CablesGridAttributes(); }
            }
        }
        else {
            for(int i=0; i<A.cableGrid.GetLength(0); i++) {
                for(int j=0; j<A.cableGrid.GetLength(1); j++) { 
                    A.cableGrid[i,j].hasCable = false; 
                    A.cableGrid[i,j].numbers.Clear(); 
                }
            }
        }
        
        Transform[,] jointsGrid = A.jointsData.jointsGrid;
        float   subJointLength  = Constants.jointDistance/2;
        for(int i=0; i<A.cables.Count; i++) {
            if(A.cables[i].gameObject.activeSelf == false) { continue; }
            Vector2 distanceFromTopLeftJoint = new Vector2(A.cables[i].position.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - A.cables[i].position.y);
            Index2D gridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2);
            if(gridIndex.x >= A.cableGrid.GetLength(0) || gridIndex.x < 0 || gridIndex.y >= A.cableGrid.GetLength(1) || gridIndex.y < 0) { continue; }
            A.cableGrid[gridIndex.x,gridIndex.y].ChangeAttributes(true, i);
        }
        A.cachedJointsGrid = A.jointsData.jointsGrid;

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

    }
    public void InitializeCachedMouseGridIndex() {
        Transform[,] jointsGrid = A.jointsData.jointsGrid;
        float   subJointLength  = Constants.jointDistance/2;
        Vector2 distanceFromTopLeftJoint = new Vector2(A.mouse.position.value.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - A.mouse.position.value.y);
        A.cachedMouseGridIndex = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        A.cachedMouseGridIndex = new Index2D(Math.Clamp(A.cachedMouseGridIndex.y, 0, jointsGrid.GetLength(0)-1), Math.Clamp(A.cachedMouseGridIndex.x, 0, jointsGrid.GetLength(1)-1));
    }

    public IEnumerator ModifyCablesOnInteract() {
        yield return new WaitForSeconds(0.01f);

        Transform[,] jointsGrid = A.jointsData.jointsGrid;
        float   subJointLength  = Constants.jointDistance/2;
        Vector2 distanceFromTopLeftJoint = new Vector2(A.mouse.position.value.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - A.mouse.position.value.y);
        Index2D mouseGridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        mouseGridIndex          = new Index2D(Math.Clamp(mouseGridIndex.y, 0, jointsGrid.GetLength(0)-1), Math.Clamp(mouseGridIndex.x, 0, jointsGrid.GetLength(1)-1));

        if(A.cachedMouseGridIndex != mouseGridIndex && !A.cableGrid[mouseGridIndex.x, mouseGridIndex.y].hasCable) {
            InitializeCableGrid();
            if(A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers != null && A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers.Count > 0) {
                int cachedIndex = A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers[0];
                Debug.Log("cachedIndex: "+cachedIndex);
                if(cachedIndex < A.initialCables.Length) { Debug.Log("trying to change an initial cable. Stopped loop."); yield break; }
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
                    if(resetToIndex < A.initialCables.Length) { resetToIndex = A.initialCables.Length; }
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
                    StopCoroutine(A.plugInteractions.ModifyCableCoroutine);
                    A.plugInteractions.ModifyCableCoroutine = null;
                    yield break;
                }
            }
            InitializeCableGrid();
            A.electricalStripController.RenewAllCableGrids();
            A.intersectionController.TestForCableIntersection();
        }
        //moving into an already defined cable
        else if(A.cachedMouseGridIndex != mouseGridIndex && A.cableGrid[mouseGridIndex.x, mouseGridIndex.y].hasCable) {

                if(A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers != null && A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers.Count > 0) {
                    int index = A.cableGrid[A.cachedMouseGridIndex.x, A.cachedMouseGridIndex.y].numbers[0];
                    if(index < A.initialCables.Length) { Debug.Log("trying to change an initial cable. Stopped loop.");yield break; }
                    Debug.Log("index: "+index);
                    Transform previousCable = A.cables[index-1];
                    A.endingDirection = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject).endingDirection;
                    GenerateEndingCables(index);
                }
                RenewRotationAndIntersectionCables();
                A.cachedMouseGridIndex = mouseGridIndex;
            InitializeCableGrid();
            A.electricalStripController.RenewAllCableGrids();
            A.intersectionController.TestForCableIntersection();
        }
        


        if(A.plugInteractions.IsModifyingCables) {
            A.plugInteractions.ModifyCableCoroutine = ModifyCablesOnInteract();
            StartCoroutine(A.plugInteractions.ModifyCableCoroutine);
        }
        else { 
            if(A.plugInteractions.ModifyCableCoroutine != null) { StopCoroutine(A.plugInteractions.ModifyCableCoroutine); }
            A.plugInteractions.ModifyCableCoroutine = null;
        }
    }

    private Index2D TestForIntersections(Index2D index2D, Directions direction) {
        int[,] plugsGrid = A.electricalStripData.plugsGrid;
        switch(direction) {
            case Directions.Up:
                for(int i=index2D.x-1; i>=0; i--) {
                    if(plugsGrid[i, index2D.y] == A.plug.id) { return index2D; }
                    if(A.cableGrid[i, index2D.y].hasCable) { return index2D; }
                }
                break;
            case Directions.Down:
                for(int i=index2D.x+1; i<A.cableGrid.GetLength(0); i++) {
                    if(plugsGrid[i, index2D.y] == A.plug.id) { return index2D; }
                    if(A.cableGrid[i, index2D.y].hasCable) { return index2D; }
                }
                break;
            case Directions.Left:
                for(int i=index2D.y-1; i>=0; i--) {
                    if(plugsGrid[index2D.x, i] == A.plug.id) { return index2D; }
                    if(A.cableGrid[index2D.x, i].hasCable) { return index2D; }
                }
                break;
            case Directions.Right:
                for(int i=index2D.y+1; i<A.cableGrid.GetLength(1); i++) {
                    if(plugsGrid[index2D.x, i] == A.plug.id) { return index2D; }
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
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs[6]); break;
                case Directions.Down:
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs[7]); break;
                case Directions.Left:
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs[2]); break;
                case Directions.Right:
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs[3]); break;
                default:
                    Debug.LogError("RenewInitialCable function did not work correctly. None of the conditions were met.");
                    prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(A.cablePrefabs[3]); break;
            }

            Sprite  prefabSprite    = prefabAttributes.cableImage.sprite;
            float   prefabZRotation = prefabAttributes.zRotation;
            Vector2 prefabPivot     = prefabAttributes.pivot;
            ModifyCableValues(A.cables[0], prefabAttributes, false, 
                              prefabZRotation, Constants.straightCableSize, prefabPivot, prefabSprite);
        }
        else {
            Transform newCable;
            switch(A.startingDirection) {
                case Directions.Up:
                    newCable = Instantiate(A.cablePrefabs[6], transform).transform;
                    A.cables.Add(newCable);
                    break;
                case Directions.Down:
                    newCable = Instantiate(A.cablePrefabs[7], transform).transform;
                    A.cables.Add(newCable);
                    break;
                case Directions.Left:
                    newCable = Instantiate(A.cablePrefabs[2], transform).transform;
                    A.cables.Add(newCable);
                    break;
                case Directions.Right:
                    newCable = Instantiate(A.cablePrefabs[3], transform).transform;
                    A.cables.Add(newCable);
                    break;
            }
        }
        A.cables[0].name = "InitialCable0";
        A.cables[0].gameObject.SetActive(true);
    }

    private void RenewRotationAndIntersectionCables() {
        for(int i=A.initialCables.Length; i<A.cables.Count; i++) {
            A.cables[i].SetSiblingIndex(i);
        }
        int index = A.initialCables.Length;
        A.lastRotationCableIndex = A.initialCables.Length;
        
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
        RenewRotationAndIntersectionCables();
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
        if((!A.plug.isObstacle && A.plug.isPluggedIn) || (A.plug.isObstacle && !A.plug.obstacleAttributes.temporarilyModifiable)) { SetCablesOpacity(1f); }
        else if((!A.plug.isObstacle && !A.plug.isPluggedIn) || (A.plug.isObstacle && A.plug.obstacleAttributes.temporarilyModifiable)) {  SetCablesOpacity(Constants.cableOpacity); }
        for(int i=A.cables.Count-1; i>=0; i--) {
            if(A.cables[i].gameObject.activeSelf) { A.endingDirection = Utilities.TryGetComponent<CableChildAttributes>(A.cables[i].gameObject).endingDirection; break; }
        }
        if(A.plug.isObstacle) { ModifyCableColorsToObstacle(); }
    }


    private void TryRenewStraightCable(int index) {
        Transform  previousCable = A.cables[index-1];
        CableChildAttributes previousAttributes = Utilities.TryGetComponent<CableChildAttributes>(previousCable.gameObject);
        Directions previousEndingDirection = previousAttributes.endingDirection;
        ShadowDirections shadowDirection = GetShadowDirectionForStraightCables(previousAttributes.shadowDirection, previousAttributes.startingDirection, previousAttributes.isRotationCable);
        
        GameObject cablePrefab = GetStraightCablePrefab(shadowDirection, previousEndingDirection);
        CableChildAttributes prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(cablePrefab);
        Vector3    deltaPosition;
        if(!previousAttributes.isRotationCable) { A.debugC.Log("previous is not rotation node"); deltaPosition = Constants.jointDistance*prefabAttributes.directionMultiple; }
        else                                   { A.debugC.Log("previous is rotation node"); deltaPosition = Vector3.zero; }
        
        
        if(A.cables.Count > index) {
            A.cables[index].position = previousCable.position + deltaPosition;
            Sprite     prefabSprite = prefabAttributes.cableImage.sprite;
            float      prefabZRotation = prefabAttributes.zRotation;
            Vector2    prefabPivot = prefabAttributes.pivot;
            ModifyCableValues(A.cables[index], prefabAttributes, false, 
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
        ShadowDirections shadowDirection = GetShadowDirectionForRotationCables(previousAttributes.shadowDirection, endingDirection);
        GameObject rotationCablePrefab = GetRotationCablePrefab(shadowDirection, startingDirection, endingDirection);
        Vector3 deltaPosition = Constants.jointDistance*previousAttributes.directionMultiple;
        Vector2 placePosition = previousCable.position + deltaPosition;
        if(A.cables.Count > index) {
            CableChildAttributes prefabAttributes = Utilities.TryGetComponent<CableChildAttributes>(rotationCablePrefab);
            Sprite     prefabSprite = prefabAttributes.cableImage.sprite;
            float      prefabZRotation = prefabAttributes.zRotation;
            Vector2    prefabPivot = prefabAttributes.pivot;

            A.cables[index].position = placePosition;
            ModifyCableValues(A.cables[index], prefabAttributes, true, 
                              prefabZRotation, Constants.rotationCableSize, prefabPivot, prefabSprite);
        }
        else {
            Transform newCable = Instantiate(rotationCablePrefab, transform).transform;
            newCable.position = placePosition;
            A.cables.Add(newCable);
        }
        A.cables[index].name = "RotationCable"+index;
    }



    private GameObject GetStraightCablePrefab(ShadowDirections shadowDirection, Directions startDirection) {
        if(shadowDirection == ShadowDirections.Up) {
            if(startDirection == Directions.Left)       { return A.cablePrefabs[0]; }
            else if(startDirection == Directions.Right) { return A.cablePrefabs[1]; }
        }
        else if(shadowDirection == ShadowDirections.Down) {
            if(startDirection == Directions.Left)       { return A.cablePrefabs[2]; }
            else if(startDirection == Directions.Right) { return A.cablePrefabs[3]; }
        }
        else if(shadowDirection == ShadowDirections.Left) {
            if(startDirection == Directions.Up)        { return A.cablePrefabs[4]; }
            else if(startDirection == Directions.Down) { return A.cablePrefabs[5]; }
        }
        else if(shadowDirection == ShadowDirections.Right) {
            if(startDirection == Directions.Up)        { return A.cablePrefabs[6]; }
            else if(startDirection == Directions.Down) { return A.cablePrefabs[7]; }
        }
        Debug.LogError("GetStraightCablePrefab function did not work correctly. None of the conditions were met.");
        return A.cablePrefabs[0]; //should never get here
    }
    
    private GameObject GetRotationCablePrefab(ShadowDirections shadowDirection, Directions startDirection, Directions endDirection) {
        if(shadowDirection == ShadowDirections.In) {
            if(startDirection == Directions.Up) {
                if     (endDirection == Directions.Left)  { return A.cablePrefabs[8]; }
                else if(endDirection == Directions.Right) { return A.cablePrefabs[9]; }
            }
            else if(startDirection == Directions.Down) { 
                if     (endDirection == Directions.Left)  { return A.cablePrefabs[10]; }
                else if(endDirection == Directions.Right) { return A.cablePrefabs[11]; }
            }
            else if(startDirection == Directions.Left) { 
                if     (endDirection == Directions.Up)  { return A.cablePrefabs[12]; }
                else if(endDirection == Directions.Down) { return A.cablePrefabs[13]; }
            }
            else if(startDirection == Directions.Right) { 
                if     (endDirection == Directions.Up)  { return A.cablePrefabs[14]; }
                else if(endDirection == Directions.Down) { return A.cablePrefabs[15]; }
            }
        }
        else if(shadowDirection == ShadowDirections.Out) {
            if(startDirection == Directions.Up) {
                if     (endDirection == Directions.Left)  { return A.cablePrefabs[16]; }
                else if(endDirection == Directions.Right) { return A.cablePrefabs[17]; }
            }
            else if(startDirection == Directions.Down) { 
                if     (endDirection == Directions.Left)  { return A.cablePrefabs[18]; }
                else if(endDirection == Directions.Right) { return A.cablePrefabs[19]; }
            }
            else if(startDirection == Directions.Left) { 
                if     (endDirection == Directions.Up)  { return A.cablePrefabs[20]; }
                else if(endDirection == Directions.Down) { return A.cablePrefabs[21]; }
            }
            else if(startDirection == Directions.Right) { 
                if     (endDirection == Directions.Up)  { return A.cablePrefabs[22]; }
                else if(endDirection == Directions.Down) { return A.cablePrefabs[23]; }
            }
        }
        Debug.LogError("GetRotationCablePrefab function did not work correctly. None of the conditions were met."+
                       "\nstartDirection: "+startDirection+
                       "\nendingDirection: "+A.endingDirection);
        return A.cablePrefabs[8]; //should never get here
    }



    private ShadowDirections GetShadowDirectionForStraightCables(ShadowDirections previousShadowDirection, Directions startingDirection, bool isRotationCable) {
        if(!isRotationCable) {
            return previousShadowDirection;
        }
        else {
            if(previousShadowDirection == ShadowDirections.In) {
                if     (startingDirection == Directions.Up   ) { return ShadowDirections.Down;  }
                else if(startingDirection == Directions.Down ) { return ShadowDirections.Up;    }
                else if(startingDirection == Directions.Left ) { return ShadowDirections.Right; }
                else if(startingDirection == Directions.Right) { return ShadowDirections.Left;  }
            }
            else {
                return (ShadowDirections)startingDirection;
            }
        }
        Debug.LogError("GetShadowDirectionForStraightCables function did not work correctly. None of the conditions were met.");
        return ShadowDirections.Down; //should never get here
    }

    //does not check for rotation cables (because 2 rotation cables will never be adjacent)
    private ShadowDirections GetShadowDirectionForRotationCables(ShadowDirections previousShadowDirection, Directions endingDirection) {
        if(previousShadowDirection == ShadowDirections.Up) {
            if     (endingDirection == Directions.Up  ) { return ShadowDirections.In; }
            else if(endingDirection == Directions.Down) { return ShadowDirections.Out; }
        }
        else if(previousShadowDirection == ShadowDirections.Down) {
            if     (endingDirection == Directions.Up  ) { return ShadowDirections.Out; }
            else if(endingDirection == Directions.Down) { return ShadowDirections.In; }
        }
        else if(previousShadowDirection == ShadowDirections.Left) {
            if     (endingDirection == Directions.Left ) { return ShadowDirections.In; }
            else if(endingDirection == Directions.Right) { return ShadowDirections.Out; }
        }
        else if(previousShadowDirection == ShadowDirections.Right) {
            if     (endingDirection == Directions.Left ) { return ShadowDirections.Out; }
            else if(endingDirection == Directions.Right) { return ShadowDirections.In; }
        }
        Debug.LogError("GetShadowDirectionForRotationCables function did not work correctly. None of the conditions were met."+
                       "\npreviousShadowDirection: "+previousShadowDirection+
                       "\nendingDirection: "+endingDirection);
        return ShadowDirections.In; //should never get here
    }


    private void ModifyCableValues(Transform currentCable, CableChildAttributes newAttributes, bool isRotationCable,
                                   float newZRotation, Vector2 newSize, Vector2 newPivot, Sprite newSprite) {
        
        Utilities.TryGetComponentInChildren<Image>(currentCable.gameObject).overrideSprite = newSprite;
        currentCable.rotation = Quaternion.Euler(0, 0, newZRotation);
        Utilities.TryGetComponentInChildren<RectTransform>(currentCable.gameObject).sizeDelta = newSize;
        
        RectTransform cableRectTransform = Utilities.TryGetComponentInChildren<RectTransform>(currentCable.gameObject);
        cableRectTransform.pivot = newPivot;

        CableChildAttributes currentAttributes = Utilities.TryGetComponent<CableChildAttributes>(currentCable.gameObject);
        InheritCableAttributes(currentAttributes, newAttributes, isRotationCable);
    }

    private void InheritCableAttributes(CableChildAttributes receiver, CableChildAttributes provider, bool isRotationCable) {
        receiver.isRotationCable   = isRotationCable;
        receiver.cableType         = provider.cableType;
        receiver.cableImage        = provider.cableImage;
        receiver.zRotation         = provider.zRotation;
        receiver.pivot             = provider.pivot;
        receiver.shadowDirection   = provider.shadowDirection;
        receiver.startingDirection = provider.startingDirection;
        receiver.endingDirection   = provider.endingDirection;
        receiver.directionMultiple = provider.directionMultiple;
    }

    public void SetCablesOpacity(float opacity) {
        CanvasGroup canvasGroup = Utilities.TryGetComponent<CanvasGroup>(gameObject);
        canvasGroup.alpha = opacity;
    }

    private void ModifyCableColorsToObstacle() {
        foreach(Transform cable in A.cables) {
            Image cableImage = Utilities.TryGetComponentInChildren<Image>(cable.gameObject);
            cableImage.color = new Color(Constants.obstacleCableColor.r,
                                         Constants.obstacleCableColor.g,
                                         Constants.obstacleCableColor.b,
                                         cableImage.color.a);
        }
    }
}
