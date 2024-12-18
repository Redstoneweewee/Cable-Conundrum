using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//[ExecuteInEditMode]
public class ElectricalStripSizeController : MonoBehaviour, IDragHandler, IBeginDragHandler {
    private ElectricalStripData D;

    // Start is called before the first frame update
    void Awake() {
        Initialize();
    }

    // Update is called once per frame
    void Update() {
        //DebugC.Log(Mouse.current.position.value);
        D.rectangularTransform = Utilities.TryGetComponent<RectTransform>(D.backgroundVisual);
        //Vector2 newSize = new Vector2((LevelResizeGlobal.instance.electricalStripBaseSize.x + LevelResizeGlobal.instance.electricalStripSeparatorSize)*D.gridsModifier.width  + LevelResizeGlobal.instance.electricalStripSeparatorSize, 
        //                              (LevelResizeGlobal.instance.electricalStripBaseSize.y + LevelResizeGlobal.instance.electricalStripSeparatorSize)*D.gridsModifier.height + 2*LevelResizeGlobal.instance.electricalStripSeparatorSize + LevelResizeGlobal.instance.powerSwitchBaseSize.y);
        //if(D.size != newSize || D.resetBoard) {
        //    RenewSockets();
        //    MovePowerSwitch();
        //}
    }

    public void Initialize() {
        D = Utilities.TryGetComponent<ElectricalStripData>(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if(!D.temporarilyModifiable) { return; }
        D.cachedMousePosition = D.mouse.position.value;
    }
    public void OnDrag(PointerEventData eventData) {
        if(!D.temporarilyModifiable) { return; }
        if(math.abs(D.cachedMousePosition.x - D.mouse.position.value.x) > LevelResizeGlobal.instance.electricalStripBaseSize.x) {
            if(D.mouse.position.value.x > D.cachedMousePosition.x) { ModifySize(Directions.Right); }
            else                                               { ModifySize(Directions.Left); }
            D.cachedMousePosition.x = D.mouse.position.value.x;
        }
        else if(math.abs(D.cachedMousePosition.y - D.mouse.position.value.y) > LevelResizeGlobal.instance.electricalStripBaseSize.y) {
            if(D.mouse.position.value.y > D.cachedMousePosition.y) { ModifySize(Directions.Up); }
            else                                               { ModifySize(Directions.Down); }
            D.cachedMousePosition.y = D.mouse.position.value.y;
        }
    }


    private void ModifySize(Directions direction) {
        switch(direction) {
            case Directions.Up:
                if(D.gridsModifier.height < 6) { D.gridsModifier.height++; }
                break;
            case Directions.Down:
                if(D.gridsModifier.height > 1) { D.gridsModifier.height--; }
                break;
            case Directions.Left:
                if(D.gridsModifier.width > 1) { D.gridsModifier.width--; }
                break;
            case Directions.Right:
                if(D.gridsModifier.width < 10) { D.gridsModifier.width++; }
                break;
        }
    }

    public void ModifyBackgroundVisual() {
        Vector2 newSize = new Vector2((LevelResizeGlobal.staticElectricalStripBaseSize.x + LevelResizeGlobal.staticElectricalStripSeparatorDistance)*D.gridsModifier.width  + LevelResizeGlobal.staticElectricalStripSeparatorDistance, 
                                      (LevelResizeGlobal.staticElectricalStripBaseSize.y + LevelResizeGlobal.staticElectricalStripSeparatorDistance)*D.gridsModifier.height + 2*LevelResizeGlobal.staticElectricalStripSeparatorDistance + LevelResizeGlobal.staticPowerSwitchBaseSize.y);
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        D.rectangularTransform.sizeDelta = newSize;
        D.rectangularTransform.position = new Vector3(center.x, center.y+(LevelResizeGlobal.instance.electricalStripSeparatorDistance + LevelResizeGlobal.instance.powerSwitchBaseSize.y)/2, 0);
        MovePowerSwitch();
    }

    private void MovePowerSwitch() {
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        Vector2 topLeft = new Vector2(D.rectangularTransform.position.x - (D.rectangularTransform.sizeDelta.x/2*LevelResizeGlobal.instance.finalScale), D.rectangularTransform.position.y + (D.rectangularTransform.sizeDelta.y/2*LevelResizeGlobal.instance.finalScale));
        D.powerSwitch.transform.position = new Vector2(center.x, topLeft.y - (LevelResizeGlobal.instance.powerSwitchBaseSize.y/2 + LevelResizeGlobal.instance.electricalStripSeparatorDistance));
    }
    /*
    public void RenewSockets() {
        D.rectangularTransform = Utilities.TryGetComponent<RectTransform>(D.backgroundVisual);
        Vector2 newSize = new Vector2((Constants.electricalStripBaseSize.x + Constants.electricalStripSeparatorSize)*D.width  + Constants.electricalStripSeparatorSize, 
                                      (Constants.electricalStripBaseSize.y + Constants.electricalStripSeparatorSize)*D.height + 2*Constants.electricalStripSeparatorSize + Constants.powerSwitchBaseSize.y);
        D.rectangularTransform.sizeDelta = newSize;
        //socketCount = width * height;
        D.size = D.rectangularTransform.sizeDelta;
        D.resetBoard = false;

        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        //DebugC.Log(size);
        Vector2 topLeft = new Vector2(center.x - D.size.x/2, center.y + D.size.y/2);
        D.socketsGrid = new Transform[D.height, D.width];

        RenewSocketsActiveGrid();


        for(int i=0; i<D.socketsParent.transform.childCount; i++) {
            D.socketsParent.transform.GetChild(i).gameObject.SetActive(false);
        }

        int childCount = D.socketsParent.transform.childCount;
        int index = 0;
        for(int i=0; i<D.socketsGrid.GetLength(0); i++) {
            for(int j=0; j<D.socketsGrid.GetLength(1); j++) {
                if(index >= childCount) {
                    GameObject newSocket = Instantiate(D.socketPrefab, D.socketsParent.transform);
                    newSocket.name = "Socket"+(index+1);
                    D.socketsGrid[i, j] = newSocket.transform;
                    Utilities.TryGetComponent<SocketAttributes>(newSocket).id = new Index2D(i, j);
                }
                else {
                    D.socketsGrid[i, j] = D.socketsParent.transform.GetChild(index);
                    foreach(Transform child in Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).childrenTransforms) {
                        child.gameObject.SetActive(true);
                    }
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).isActive = true;
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).id = new Index2D(i, j);
                }
                if(D.socketsActiveGrid[i].row[j] == false) { 
                    Debug.Log($"Socket At ({i}, {j}) is inactive."); 
                    foreach(Transform child in Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).childrenTransforms) {
                        child.gameObject.SetActive(false);
                    }
                    D.socketsGrid[i, j].gameObject.SetActive(true);
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).isActive = false;
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).id = new Index2D(i, j);
                }
                index++;
            }
        }
        D.debugC.LogArray2D("", D.socketsGrid);
        for(int i=1; i<=D.height; i++) {
            for(int j=1; j<=D.width; j++) {
                D.socketsGrid[i-1,j-1].position = new Vector2(topLeft.x + D.r*(j - 0.5f) + D.s*j,
                                                                                      topLeft.y - D.r*(i - 0.5f) - D.s*i - (Constants.powerSwitchBaseSize.y + D.s));
            }
        }
    }
    */



    /*
    private void RenewSocketsActiveGrid() {
        if(D.socketsActiveGrid == null) {
            D.socketsActiveGrid.Clear();
            for(int i=0; i<D.height; i++) {
                D.socketsActiveGrid.Add(new SocketsRow(D.width));
            }
        }
        else {
            List<SocketsRow> temp = new List<SocketsRow>();
            for(int i=0; i<D.socketsActiveGrid.Count; i++) { temp.Add(new SocketsRow(D.socketsActiveGrid[i], D.width)); }

            D.socketsActiveGrid.Clear();
            for(int i=0; i<D.height; i++) {
                if(i < temp.Count) { D.socketsActiveGrid.Add(new SocketsRow(temp[i], D.width)); }
                else { D.socketsActiveGrid.Add(new SocketsRow(D.width)); }
            }
        }
        D.cachedSocketsActiveGrid = D.socketsActiveGrid;
    }
    */
}
