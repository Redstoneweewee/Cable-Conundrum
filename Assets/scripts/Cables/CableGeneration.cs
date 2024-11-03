using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CableGeneration : MonoBehaviour, IDebugC {
    private Mouse mouse = Mouse.current;
    public DebugC DebugC {set; get;}

    private Plug plug;
    private IntersectionDetector intersectionDetector;
    private PlugInteractions plugInteractions;
    private ElectricalStripController electricalStripController;
    private JointsController jointsController;
    [SerializeField] GameObject[] cablePrefabs;
    /* Cables:
    * [ [0 ]UpLeft,    [1 ]UpRight,    [2 ]DownLeft,    [3 ]DownRight,    [4 ]LeftUp,    [5 ]LeftDown,    [6 ]RightUp,    [7 ]RightDown,   ]
    * [ [8 ]InUpLeft,  [9 ]InUpRight,  [10]InDownLeft,  [11]InDownRight,  [12]InLeftUp,  [13]InLeftDown,  [14]InRightUp,  [15]InRightDown, ]
    * [ [16]OutUpLeft, [17]OutUpRight, [18]OutDownLeft, [19]OutDownRight, [20]OutLeftUp, [21]OutLeftDown, [22]OutRightUp, [23]OutRightDown ]
    * Link: https://docs.google.com/document/d/1-T7I-lNiF93s7gjlgOsbJBzwmPMfbWuQ_Jdx-Hd63DM/edit?usp=sharing
    */
    [SerializeField] GameObject[] initialCables;
    private Vector3 cachedPosition;
    private List<Transform> cables = new List<Transform>();
    public List<Transform> Cables {get{return cables;} set{cables = value;}}
    //doesn't get used/generated until the player plugs the plug into a socket
    //numbers always contain 0; keep going up to find the next cable until the size of the grid
    private CablesGridAttributes[,] cableGrid; 
    public CablesGridAttributes[,] CableGrid {get{return cableGrid;} set{cableGrid = value;}}
    [SerializeField] private Directions startingDirection = Directions.Down;
    public Directions StartingDirection {get{return startingDirection;} set{startingDirection = value;}}
    [SerializeField] private Directions endingDirection   = Directions.Down;
    private int        lastRotationCableIndex;
    private Directions cachedStartingDirection;
    private Directions cachedEndingDirection;
    private Index2D    cachedMouseGridIndex;
    private Transform[,] cachedJointsGrid;

    // Start is called before the first frame update
    void Start() {
        DebugC = DebugC.Get();
        intersectionDetector = FindObjectOfType<IntersectionDetector>();
        jointsController = FindObjectOfType<JointsController>();
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        plug = GetComponentInParent<Plug>();
        plugInteractions = GetComponentInParent<PlugInteractions>();
        //cachedPosition = transform.position;  //want to renew cables twice on start
        Initialize();
        RenewRotationAndIntersectionCables();
    }

    // Update is called once per frame
    void Update() {
        if(transform.position != cachedPosition || startingDirection != cachedStartingDirection || endingDirection != cachedEndingDirection) {
            GenerateEndingCables(lastRotationCableIndex+1);
            cachedPosition = transform.position;
            cachedStartingDirection = startingDirection;
            cachedEndingDirection = endingDirection;
        }
        
        //string text = "";
        //foreach(Transform cable in cables) {
        //    text += $"{cable.name}, ";
        //}
        //Debug.Log("cables: "+text);
        
        //Debug.Log(mouseGridIndex);
    }

    private void Initialize() {
        int endingCablesIndex = initialCables.Length;
        InitializeInitialCables();
        //RenewInitialCable();
        if(startingDirection != endingDirection) { TryRenewRotationCable(initialCables.Length, startingDirection, endingDirection); endingCablesIndex++; }
        GenerateEndingCables(endingCablesIndex);
    }

    public void ResetCableGrid() {
        for(int i=0; i<cableGrid.GetLength(0); i++) {
            for(int j=0; j<cableGrid.GetLength(1); j++) { 
                cableGrid[i,j].hasCable = false; 
                cableGrid[i,j].numbers.Clear(); 
            }
        }
        //cableGrid = new CableGridAttributes[jointsController.JointsGrid.GetLength(0), jointsController.JointsGrid.GetLength(1)];
    }
    public void InitializeCableGrid() {
        if(cachedJointsGrid == null || cachedJointsGrid != jointsController.JointsGrid) {
            cableGrid = new CablesGridAttributes[jointsController.JointsGrid.GetLength(0), jointsController.JointsGrid.GetLength(1)];
            for(int i=0; i<cableGrid.GetLength(0); i++) {
                for(int j=0; j<cableGrid.GetLength(1); j++) { cableGrid[i,j] = new CablesGridAttributes(); }
            }
        }
        else {
            for(int i=0; i<cableGrid.GetLength(0); i++) {
                for(int j=0; j<cableGrid.GetLength(1); j++) { 
                    cableGrid[i,j].hasCable = false; 
                    cableGrid[i,j].numbers.Clear(); 
                }
            }
        }
        
        Transform[,] jointsGrid = jointsController.JointsGrid;
        float   subJointLength  = Constants.jointDistance/2;
        for(int i=0; i<cables.Count; i++) {
            if(cables[i].gameObject.activeSelf == false) { continue; }
            Vector2 distanceFromTopLeftJoint = new Vector2(cables[i].position.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - cables[i].position.y);
            Index2D gridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2);
            if(gridIndex.x >= cableGrid.GetLength(0) || gridIndex.x < 0 || gridIndex.y >= cableGrid.GetLength(1) || gridIndex.y < 0) { continue; }
            cableGrid[gridIndex.x,gridIndex.y].ChangeAttributes(true, i);
        }
        cachedJointsGrid = jointsController.JointsGrid;
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //  this is not modifying correctly (updating) even after the cables have changed !!!
        //  seems to happen when the player tries to turn the cable into itself and then it all
        //  cancels out into a straight line
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        string text = "";
        for(int i=0; i<cableGrid.GetLength(0); i++) {
            text += "| ";
            for(int j=0; j<cableGrid.GetLength(1); j++) {
                if(cableGrid[i,j].hasCable == true) { text += "*  "; }
                //if(cableGrid[i,j].hasCable == true) { text += cableGrid[i,j].number + " "; }
                else { text += "-  "; }
            }
            text += " |\n";
        }
        DebugC.Log("cableGrid: \n"+text);

    }
    public void InitializeCachedMouseGridIndex() {
        Transform[,] jointsGrid = jointsController.JointsGrid;
        float   subJointLength  = Constants.jointDistance/2;
        Vector2 distanceFromTopLeftJoint = new Vector2(mouse.position.value.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - mouse.position.value.y);
        cachedMouseGridIndex = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        cachedMouseGridIndex = new Index2D(Math.Clamp(cachedMouseGridIndex.y, 0, jointsGrid.GetLength(0)-1), Math.Clamp(cachedMouseGridIndex.x, 0, jointsGrid.GetLength(1)-1));
    }

    public IEnumerator ModifyCablesOnInteract() {
        yield return new WaitForSeconds(0.01f);

        Transform[,] jointsGrid = jointsController.JointsGrid;
        //Vector2 jointsGridSize   = new Vector2((jointsGrid.GetLength(1)-1)*Constants.jointDistance, (jointsGrid.GetLength(0)-1)*Constants.jointDistance);
        float   subJointLength  = Constants.jointDistance/2;
        Vector2 distanceFromTopLeftJoint = new Vector2(mouse.position.value.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - mouse.position.value.y);
        Index2D mouseGridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        mouseGridIndex          = new Index2D(Math.Clamp(mouseGridIndex.y, 0, jointsGrid.GetLength(0)-1), Math.Clamp(mouseGridIndex.x, 0, jointsGrid.GetLength(1)-1));

        //moving into an empty location (only detects cables from this plug)
        if(cachedMouseGridIndex != mouseGridIndex && !cableGrid[mouseGridIndex.x, mouseGridIndex.y].hasCable) {
            InitializeCableGrid();
            if(cableGrid[cachedMouseGridIndex.x, cachedMouseGridIndex.y].numbers != null && cableGrid[cachedMouseGridIndex.x, cachedMouseGridIndex.y].numbers.Count > 0) {
                int cachedIndex = cableGrid[cachedMouseGridIndex.x, cachedMouseGridIndex.y].numbers[0];
                Debug.Log("cachedIndex: "+cachedIndex);
                if(cachedIndex < initialCables.Length) { Debug.Log("trying to change an initial cable. Stopped loop."); yield break; }
                //int index = cableGrid[mouseGridIndex.x, mouseGridIndex.y].number;
                //if(index < initialCables.Length) { index = initialCables.Length-1; }
                //Debug.Log("Aindex: "+index);
                Index2D deltaGridIndex = new Index2D(mouseGridIndex.x - cachedMouseGridIndex.x, mouseGridIndex.y - cachedMouseGridIndex.y);
                int previousIndex = cachedIndex - 1;
                if(previousIndex < 0) { previousIndex = 0; }
                if(deltaGridIndex.x == -1)      { endingDirection = Directions.Up; }
                else if(deltaGridIndex.x == 1)  { endingDirection = Directions.Down; }
                else if(deltaGridIndex.y == -1) { endingDirection = Directions.Left; }
                else if(deltaGridIndex.y == 1)  { endingDirection = Directions.Right; }
                Directions startDirection = cables[previousIndex].GetComponent<CableAttributes>().EndingDirection;
                Index2D gridIntersectionIndex = TestForIntersections(cachedMouseGridIndex, endingDirection);
                if(gridIntersectionIndex != new Index2D(-1, -1)) {
                    DebugC.Log($"tried to create a loop. stopping cable generation. starting index: ({mouseGridIndex.x}, {mouseGridIndex.y}), direction: {endingDirection}, intersectionindex: ({gridIntersectionIndex.x}, {gridIntersectionIndex.y})");
                    int resetToIndex = cableGrid[gridIntersectionIndex.x, gridIntersectionIndex.y].numbers[0];
                    if(resetToIndex < initialCables.Length) { resetToIndex = initialCables.Length; }
                    Transform resetToCable = cables[resetToIndex];
                    endingDirection = resetToCable.GetComponent<CableAttributes>().EndingDirection;
                    GenerateEndingCables(resetToIndex);
                    RenewRotationAndIntersectionCables();
                    cachedMouseGridIndex = mouseGridIndex;
                }
                else if(!DirectionsHaveError(startDirection, endingDirection)) {
                    if(deltaGridIndex.x == -1) { //moved up
                        DebugC.Log("moved up");
                        TryGenerateCable(previousIndex, Directions.Up);
                    }
                    else if(deltaGridIndex.x == 1) { //moved down
                        DebugC.Log("moved down");
                        TryGenerateCable(previousIndex, Directions.Down);
                    }
                    else if(deltaGridIndex.y == -1) { //moved left
                        DebugC.Log("moved left");
                        TryGenerateCable(previousIndex, Directions.Left);
                    }
                    else if(deltaGridIndex.y == 1) { //moved right
                        DebugC.Log("moved right");
                        TryGenerateCable(previousIndex, Directions.Right);
                    }
                    else {
                        Debug.LogWarning("Moved mouse too quickly");
                    }
                    cachedMouseGridIndex = mouseGridIndex;
                }
                else {
                    Debug.LogWarning($"Impossible directions. Starting: {startDirection}, Ending: {endingDirection}");
                    for(int i=cables.Count-1; i>=0; i--) {
                        if(cables[i].gameObject.activeSelf) { 
                            endingDirection = cables[i].GetComponent<CableAttributes>().EndingDirection; 
                            break;
                        }
                    }
                    StopCoroutine(plugInteractions.ModifyCableCoroutine);
                    plugInteractions.ModifyCableCoroutine = null;
                    yield break;
                }
            }
            InitializeCableGrid();
            electricalStripController.RenewAllCableGrids();
            intersectionDetector.TestForCableIntersection();
        }
        //moving into an already defined cable
        else if(cachedMouseGridIndex != mouseGridIndex && cableGrid[mouseGridIndex.x, mouseGridIndex.y].hasCable) {
            //if(Math.Abs(cableGrid[mouseGridIndex.x, mouseGridIndex.y].number-cableGrid[cachedMouseGridIndex.x, cachedMouseGridIndex.y].number) > 2) {
            //    Debug.Log("tried to create a loop. stopping cable generation.");
            //    StopCoroutine(plugInteractions.ModifyCableCoroutine);
            //    plugInteractions.ModifyCableCoroutine = null;
            //    yield break;
            //}
            //else {
                if(cableGrid[cachedMouseGridIndex.x, cachedMouseGridIndex.y].numbers != null && cableGrid[cachedMouseGridIndex.x, cachedMouseGridIndex.y].numbers.Count > 0) {
                    int index = cableGrid[cachedMouseGridIndex.x, cachedMouseGridIndex.y].numbers[0];
                    if(index < initialCables.Length) { Debug.Log("trying to change an initial cable. Stopped loop.");yield break; }
                    Debug.Log("index: "+index);
                    Transform previousCable = cables[index-1];
                    endingDirection = previousCable.GetComponent<CableAttributes>().EndingDirection;
                    GenerateEndingCables(index);
                }
                RenewRotationAndIntersectionCables();
                cachedMouseGridIndex = mouseGridIndex;
            //}
            InitializeCableGrid();
            electricalStripController.RenewAllCableGrids();
            intersectionDetector.TestForCableIntersection();
        }
        
        //Debug.Log("interacting with cables");


        if(plugInteractions.IsModifyingCables) {
            plugInteractions.ModifyCableCoroutine = ModifyCablesOnInteract();
            StartCoroutine(plugInteractions.ModifyCableCoroutine);
        }
        else { 
            if(plugInteractions.ModifyCableCoroutine != null) { StopCoroutine(plugInteractions.ModifyCableCoroutine); }
            plugInteractions.ModifyCableCoroutine = null;
        }
    }

    private Index2D TestForIntersections(Index2D index2D, Directions direction) {
        int[,] plugsGrid = electricalStripController.PlugsGrid;
        switch(direction) {
            case Directions.Up:
                for(int i=index2D.x-1; i>=0; i--) {
                    if(plugsGrid[i, index2D.y] == plug.Id) { return index2D; }
                    if(cableGrid[i, index2D.y].hasCable) { return index2D; }
                    //else if(!cableGrid[i, index2D.y].hasCable) { return new Index2D(-1, -1); }
                }
                break;
            case Directions.Down:
                for(int i=index2D.x+1; i<cableGrid.GetLength(0); i++) {
                    if(plugsGrid[i, index2D.y] == plug.Id) { return index2D; }
                    if(cableGrid[i, index2D.y].hasCable) { return index2D; }
                    //else if(!cableGrid[i, index2D.y].hasCable) { return new Index2D(-1, -1); }
                }
                break;
            case Directions.Left:
                for(int i=index2D.y-1; i>=0; i--) {
                    if(plugsGrid[index2D.x, i] == plug.Id) { return index2D; }
                    if(cableGrid[index2D.x, i].hasCable) { return index2D; }
                    //else if(!cableGrid[index2D.x, i].hasCable) { return new Index2D(-1, -1); }
                }
                break;
            case Directions.Right:
                for(int i=index2D.y+1; i<cableGrid.GetLength(1); i++) {
                    if(plugsGrid[index2D.x, i] == plug.Id) { return index2D; }
                    if(cableGrid[index2D.x, i].hasCable) { return index2D; }
                    //else if(!cableGrid[index2D.x, i].hasCable) { return new Index2D(-1, -1); }
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
        //Transform[] initialCables = GetComponentsInChildren<Transform>();
        foreach(GameObject initialCable in initialCables) {
            cables.Add(initialCable.transform);
            DebugC.Log($"{initialCable.name} added to cables list for {transform.name}");
        }
    }
    private void RenewInitialCable() {
        if(cables.Count > 0) {
            CableAttributes prefabAttributes;
            switch(startingDirection) {
                case Directions.Up:
                    prefabAttributes = cablePrefabs[6].GetComponent<CableAttributes>(); break;
                case Directions.Down:
                    prefabAttributes = cablePrefabs[7].GetComponent<CableAttributes>(); break;
                case Directions.Left:
                    prefabAttributes = cablePrefabs[2].GetComponent<CableAttributes>(); break;
                case Directions.Right:
                    prefabAttributes = cablePrefabs[3].GetComponent<CableAttributes>(); break;
                default:
                    Debug.LogError("RenewInitialCable function did not work correctly. None of the conditions were met.");
                    prefabAttributes = cablePrefabs[3].GetComponent<CableAttributes>(); break;
            }

            Sprite  prefabSprite    = prefabAttributes.CableImage.sprite;
            float   prefabZRotation = prefabAttributes.ZRotation;
            Vector2 prefabPivot     = prefabAttributes.Pivot;
            ModifyCableValues(cables[0], prefabAttributes, false, 
                              prefabZRotation, Constants.straightCableSize, prefabPivot, prefabSprite);
        }
        else {
            Transform newCable;
            switch(startingDirection) {
                case Directions.Up:
                    newCable = Instantiate(cablePrefabs[6], transform).transform;
                    cables.Add(newCable);
                    break;
                case Directions.Down:
                    newCable = Instantiate(cablePrefabs[7], transform).transform;
                    cables.Add(newCable);
                    break;
                case Directions.Left:
                    newCable = Instantiate(cablePrefabs[2], transform).transform;
                    cables.Add(newCable);
                    break;
                case Directions.Right:
                    newCable = Instantiate(cablePrefabs[3], transform).transform;
                    cables.Add(newCable);
                    break;
            }
        }
        cables[0].name = "InitialCable0";
        cables[0].gameObject.SetActive(true);
    }

    private void RenewRotationAndIntersectionCables() {
        for(int i=initialCables.Length; i<cables.Count; i++) {
            cables[i].SetSiblingIndex(i);
        }
        int index = initialCables.Length;
        lastRotationCableIndex = initialCables.Length;
        
        while(index < cables.Count) {
            int nextRotationCableIndex = index;
            //finds the next rotation cable and sets nextIndex to its index
            for(int i=index; i<cables.Count; i++) {
                
                if((cables[i].GetComponent<CableAttributes>().IsRotationCable || cables[i].GetComponent<CableAttributes>().IsIntersectionCable ) && cables[i].gameObject.activeSelf) { 
                    nextRotationCableIndex = i; 
                    lastRotationCableIndex = i;
                    cables[i].SetSiblingIndex(i+1);
                    break; 
                }
                if(i == cables.Count - 1) { return; }
            }
            //renew straight cables up until the rotation cable
            //for(int i=index; i<nextRotationCableIndex; i++) {
            //    TryRenewStraightCable(i); 
            //}

            //sets index to one after the rotation cable to search for next rotation cable
            index = nextRotationCableIndex + 1;
        }
    }

    //Generates a straight cable (and a rotation cable if necessary) based on the grid attribute of the previous joint that the player was hovering over.
    //Uses the previous cable as a reference.
    private void TryGenerateCable(int previousIndex, Directions newDirection) {
        //Debug.Log("previousIndex: "+previousIndex);
        Transform previousCable;
        CableAttributes previousAttributes;
        if(!cables[previousIndex].GetComponent<CableAttributes>().IsRotationCable) {
            previousCable = cables[previousIndex];
            previousAttributes = previousCable.GetComponent<CableAttributes>();
        }
        else {
            previousIndex -= 1;
            previousCable = cables[previousIndex];
            previousAttributes = previousCable.GetComponent<CableAttributes>();
        }
        //going straight
        if(previousAttributes.EndingDirection == newDirection) {
            TryRenewStraightCable(previousIndex+1);
        }
        //turning
        else if(previousAttributes.EndingDirection != newDirection) {
            TryRenewRotationCable(previousIndex+1, previousAttributes.EndingDirection, newDirection);
            DebugC.Log("renewed rotation cable, direction: "+newDirection);
            //TryRenewStraightCable(previousIndex+2);
            //generate rotation cable
            //generate straight cable
        }
        GenerateEndingCables(previousIndex+2);
        RenewRotationAndIntersectionCables();
    }


    private void GenerateEndingCables(int index) {
        for(int i=index; i<cables.Count; i++) { cables[i].gameObject.SetActive(false); }
        //------------------- it sometimes says cables[index-1] is out of bounds -------------------
        Transform previousCable = cables[index-1];
        Vector2 currentPosition = previousCable.position;
        switch(endingDirection) {
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
        if((!plug.IsObstacle && plug.isPluggedIn) || (plug.IsObstacle && !plug.Obstacle.TemporarilyModifiable)) { SetCablesOpacity(1f); }
        else if((!plug.IsObstacle && !plug.isPluggedIn) || (plug.IsObstacle && plug.Obstacle.TemporarilyModifiable)) {  SetCablesOpacity(Constants.cableOpacity); }
        for(int i=cables.Count-1; i>=0; i--) {
            if(cables[i].gameObject.activeSelf) { endingDirection = cables[i].GetComponent<CableAttributes>().EndingDirection; break; }
        }
        if(plug.IsObstacle) { ModifyCableColorsToObstacle(); }
    }


    private void TryRenewStraightCable(int index) {
        Transform  previousCable = cables[index-1];
        CableAttributes previousAttributes = previousCable.GetComponent<CableAttributes>();
        Directions previousEndingDirection = previousAttributes.EndingDirection;
        ShadowDirections shadowDirection = GetShadowDirectionForStraightCables(previousAttributes.ShadowDirection, previousAttributes.StartingDirection, previousAttributes.IsRotationCable);
        
        GameObject cablePrefab = GetStraightCablePrefab(shadowDirection, previousEndingDirection);
        CableAttributes prefabAttributes = cablePrefab.GetComponent<CableAttributes>();
        Vector3    deltaPosition;
        if(!previousAttributes.IsRotationCable) { DebugC.Log("previous is not rotation node"); deltaPosition = Constants.jointDistance*prefabAttributes.DirectionMultiple; }
        else                                   { DebugC.Log("previous is rotation node"); deltaPosition = Vector3.zero; }
        
        
        if(cables.Count > index) {
            cables[index].position = previousCable.position + deltaPosition;
            Sprite     prefabSprite = prefabAttributes.CableImage.sprite;
            float      prefabZRotation = prefabAttributes.ZRotation;
            Vector2    prefabPivot = prefabAttributes.Pivot;
            ModifyCableValues(cables[index], prefabAttributes, false, 
                            prefabZRotation, Constants.straightCableSize, prefabPivot, prefabSprite);
            cables[index].gameObject.SetActive(true);
        }
        else {
            cables.Add(Instantiate(cablePrefab, transform).transform);
            cables[index].position = cables[index-1].position + deltaPosition;
        }
        cables[index].name = "Cable"+index;
    }

    private void TryRenewRotationCable(int index, Directions startingDirection, Directions endingDirection) {
        Transform previousCable = cables[index-1];
        CableAttributes previousAttributes = previousCable.GetComponent<CableAttributes>();
        ShadowDirections shadowDirection = GetShadowDirectionForRotationCables(previousAttributes.ShadowDirection, endingDirection);
        GameObject rotationCablePrefab = GetRotationCablePrefab(shadowDirection, startingDirection, endingDirection);
        Vector3 deltaPosition = Constants.jointDistance*previousAttributes.DirectionMultiple;
        Vector2 placePosition = previousCable.position + deltaPosition;
        if(cables.Count > index) {
            CableAttributes prefabAttributes = rotationCablePrefab.GetComponent<CableAttributes>();
            Sprite     prefabSprite = prefabAttributes.CableImage.sprite;
            float      prefabZRotation = prefabAttributes.ZRotation;
            Vector2    prefabPivot = prefabAttributes.Pivot;

            cables[index].position = placePosition;
            ModifyCableValues(cables[index], prefabAttributes, true, 
                              prefabZRotation, Constants.rotationCableSize, prefabPivot, prefabSprite);
        }
        else {
            Transform newCable = Instantiate(rotationCablePrefab, transform).transform;
            newCable.position = placePosition;
            cables.Add(newCable);
        }
        cables[index].name = "RotationCable"+index;
    }



    private GameObject GetStraightCablePrefab(ShadowDirections shadowDirection, Directions startDirection) {
        if(shadowDirection == ShadowDirections.Up) {
            if(startDirection == Directions.Left)       { return cablePrefabs[0]; }
            else if(startDirection == Directions.Right) { return cablePrefabs[1]; }
        }
        else if(shadowDirection == ShadowDirections.Down) {
            if(startDirection == Directions.Left)       { return cablePrefabs[2]; }
            else if(startDirection == Directions.Right) { return cablePrefabs[3]; }
        }
        else if(shadowDirection == ShadowDirections.Left) {
            if(startDirection == Directions.Up)        { return cablePrefabs[4]; }
            else if(startDirection == Directions.Down) { return cablePrefabs[5]; }
        }
        else if(shadowDirection == ShadowDirections.Right) {
            if(startDirection == Directions.Up)        { return cablePrefabs[6]; }
            else if(startDirection == Directions.Down) { return cablePrefabs[7]; }
        }
        Debug.LogError("GetStraightCablePrefab function did not work correctly. None of the conditions were met.");
        return cablePrefabs[0]; //should never get here
    }
    
    private GameObject GetRotationCablePrefab(ShadowDirections shadowDirection, Directions startDirection, Directions endDirection) {
        if(shadowDirection == ShadowDirections.In) {
            if(startDirection == Directions.Up) {
                if     (endDirection == Directions.Left)  { return cablePrefabs[8]; }
                else if(endDirection == Directions.Right) { return cablePrefabs[9]; }
            }
            else if(startDirection == Directions.Down) { 
                if     (endDirection == Directions.Left)  { return cablePrefabs[10]; }
                else if(endDirection == Directions.Right) { return cablePrefabs[11]; }
            }
            else if(startDirection == Directions.Left) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs[12]; }
                else if(endDirection == Directions.Down) { return cablePrefabs[13]; }
            }
            else if(startDirection == Directions.Right) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs[14]; }
                else if(endDirection == Directions.Down) { return cablePrefabs[15]; }
            }
        }
        else if(shadowDirection == ShadowDirections.Out) {
            if(startDirection == Directions.Up) {
                if     (endDirection == Directions.Left)  { return cablePrefabs[16]; }
                else if(endDirection == Directions.Right) { return cablePrefabs[17]; }
            }
            else if(startDirection == Directions.Down) { 
                if     (endDirection == Directions.Left)  { return cablePrefabs[18]; }
                else if(endDirection == Directions.Right) { return cablePrefabs[19]; }
            }
            else if(startDirection == Directions.Left) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs[20]; }
                else if(endDirection == Directions.Down) { return cablePrefabs[21]; }
            }
            else if(startDirection == Directions.Right) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs[22]; }
                else if(endDirection == Directions.Down) { return cablePrefabs[23]; }
            }
        }
        Debug.LogError("GetRotationCablePrefab function did not work correctly. None of the conditions were met."+
                       "\nstartDirection: "+startDirection+
                       "\nendingDirection: "+endingDirection);
        return cablePrefabs[8]; //should never get here
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


    private void ModifyCableValues(Transform currentCable, CableAttributes newAttributes, bool isRotationCable,
                                   float newZRotation, Vector2 newSize, Vector2 newPivot, Sprite newSprite) {
        
        currentCable.GetComponentInChildren<Image>().overrideSprite = newSprite;
        currentCable.rotation = Quaternion.Euler(0, 0, newZRotation);
        currentCable.GetComponentInChildren<RectTransform>().sizeDelta = newSize;
        
        RectTransform cableRectTransform = currentCable.GetComponentInChildren<RectTransform>();
        cableRectTransform.pivot = newPivot;

        CableAttributes currentAttributes = currentCable.GetComponent<CableAttributes>();
        InheritCableAttributes(currentAttributes, newAttributes, isRotationCable);
    }

    private void InheritCableAttributes(CableAttributes receiver, CableAttributes provider, bool isRotationCable) {
        receiver.IsRotationCable   = isRotationCable;
        receiver.CableType         = provider.CableType;
        receiver.CableImage        = provider.CableImage;
        receiver.ZRotation         = provider.ZRotation;
        receiver.Pivot             = provider.Pivot;
        receiver.ShadowDirection   = provider.ShadowDirection;
        receiver.StartingDirection = provider.StartingDirection;
        receiver.EndingDirection   = provider.EndingDirection;
        receiver.DirectionMultiple = provider.DirectionMultiple;
    }

    public void SetCablesOpacity(float opacity) {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = opacity;
    }

    private void ModifyCableColorsToObstacle() {
        foreach(Transform cable in cables) {
            Image cableImage = cable.GetComponentInChildren<Image>();
            cableImage.color = new Color(Constants.obstacleCableColor.r,
                                         Constants.obstacleCableColor.g,
                                         Constants.obstacleCableColor.b,
                                         cableImage.color.a);
        }
    }
}
