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
    void Start() {
        D = GetComponent<ElectricalStripData>();
    }

    // Update is called once per frame
    void Update() {
        //DebugC.Log(Mouse.current.position.value);
        D.rectangularTransform = D.backgroundVisual.GetComponent<RectTransform>();
        Vector2 newSize = new Vector2((Constants.electricalStripBaseSize.x + Constants.electricalStripSeparatorSize)*D.width  + Constants.electricalStripSeparatorSize, 
                                      (Constants.electricalStripBaseSize.y + Constants.electricalStripSeparatorSize)*D.height + 2*Constants.electricalStripSeparatorSize + Constants.powerSwitchBaseSize.y);
        if(D.size != newSize || D.resetBoard) {
            RenewSockets();
            MovePowerSwitch();
        }
    }


    public void OnBeginDrag(PointerEventData eventData) {
        if(!D.temporarilyModifiable) { return; }
        D.cachedMousePosition = D.mouse.position.value;
    }
    public void OnDrag(PointerEventData eventData) {
        if(!D.temporarilyModifiable) { return; }
        if(math.abs(D.cachedMousePosition.x - D.mouse.position.value.x) > Constants.electricalStripBaseSize.x) {
            if(D.mouse.position.value.x > D.cachedMousePosition.x) { ModifySize(Directions.Right); }
            else                                               { ModifySize(Directions.Left); }
            D.cachedMousePosition.x = D.mouse.position.value.x;
        }
        else if(math.abs(D.cachedMousePosition.y - D.mouse.position.value.y) > Constants.electricalStripBaseSize.y) {
            if(D.mouse.position.value.y > D.cachedMousePosition.y) { ModifySize(Directions.Up); }
            else                                               { ModifySize(Directions.Down); }
            D.cachedMousePosition.y = D.mouse.position.value.y;
        }
    }


    private void ModifySize(Directions direction) {
        switch(direction) {
            case Directions.Up:
                if(D.height < 6) { D.height++; }
                break;
            case Directions.Down:
                if(D.height > 1) { D.height--; }
                break;
            case Directions.Left:
                if(D.width > 1) { D.width--; }
                break;
            case Directions.Right:
                if(D.width < 10) { D.width++; }
                break;
        }
    }


    public void RenewSockets() {
        D.rectangularTransform = D.backgroundVisual.GetComponent<RectTransform>();
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
        //DebugC.Log("width: "+electricalStripController.SocketsGrid.GetLength(0));
        //DebugC.Log("height: "+electricalStripController.SocketsGrid.GetLength(1));
        for(int i=0; i<D.socketsGrid.GetLength(0); i++) {
            for(int j=0; j<D.socketsGrid.GetLength(1); j++) {
                if(index >= childCount) {
                    GameObject newSocket = Instantiate(D.socketPrefab, D.socketsParent.transform);
                    newSocket.name = "Socket"+(index+1);
                    D.socketsGrid[i, j] = newSocket.transform;
                    newSocket.GetComponent<SocketAttributes>().id = new Index2D(i, j);
                }
                else {
                    D.socketsGrid[i, j] = D.socketsParent.transform.GetChild(index);
                    foreach(Transform child in D.socketsGrid[i, j].GetComponent<SocketAttributes>().childrenTransforms) {
                        child.gameObject.SetActive(true);
                    }
                    D.socketsGrid[i, j].GetComponent<SocketAttributes>().isActive = true;
                    D.socketsGrid[i, j].GetComponent<SocketAttributes>().id = new Index2D(i, j);
                }
                if(D.socketsActiveGrid[i].row[j] == false) { 
                    Debug.Log($"Socket At ({i}, {j}) is inactive."); 
                    foreach(Transform child in D.socketsGrid[i, j].GetComponent<SocketAttributes>().childrenTransforms) {
                        child.gameObject.SetActive(false);
                    }
                    D.socketsGrid[i, j].gameObject.SetActive(true);
                    D.socketsGrid[i, j].GetComponent<SocketAttributes>().isActive = false;
                    D.socketsGrid[i, j].GetComponent<SocketAttributes>().id = new Index2D(i, j);
                }
                index++;
            }
        }
        D.debugC.LogArray2D("", D.socketsGrid);
        //Utilities.LogList<Transform>("sockets before: ", sockets);
        //sockets.RemoveAt(0);
        //Utilities.LogList<Transform>("sockets after: ", sockets);
        for(int i=1; i<=D.height; i++) {
            for(int j=1; j<=D.width; j++) {
                //sockets[count].position = topLeft;
                //DebugC.Log(r*(i - 0.5f));
                //DebugC.Log(r*(i - 0.5f) + s*i);
                D.socketsGrid[i-1,j-1].position = new Vector2(topLeft.x + D.r*(j - 0.5f) + D.s*j,
                                                                                      topLeft.y - D.r*(i - 0.5f) - D.s*i - (Constants.powerSwitchBaseSize.y + D.s));
            }
        }
        //DebugC.Log($"sockets count: {sockets.Count()}, actual count: {socketCount}");
        //sockets.RemoveRange(socketCount,sockets.Count()-socketCount);

        //DebugC.Log($"sockets count: {sockets.Count()}, actual count: {socketCount}");
    }

    private void MovePowerSwitch() {
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        Vector2 topLeft = new Vector2(center.x - D.size.x/2, center.y + D.size.y/2);
        D.powerSwitch.transform.position = new Vector2(center.x, topLeft.y - Constants.powerSwitchBaseSize.y/2 - D.s);
    }

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
}
