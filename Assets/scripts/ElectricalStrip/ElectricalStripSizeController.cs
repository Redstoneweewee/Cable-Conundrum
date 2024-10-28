using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[Serializable]
public class SocketsRow {
    [SerializeField] private List<bool> row = new List<bool>();
    public List<bool> Row { get{return row;} set{row = value;} }
    public SocketsRow(int length) {
        for(int i=0; i<length; i++) { row.Add(true); }
    }
    public SocketsRow(SocketsRow previousSocketsRow, int max) {
        for(int i=0; i<max; i++) {
            if(i < previousSocketsRow.Row.Count) { row.Add(previousSocketsRow.Row[i]); }
            else { row.Add(true); }
        }
    }
}



[ExecuteInEditMode]
public class ElectricalStripSizeController : MonoBehaviour, IDebugC, IDragHandler, IBeginDragHandler {
    public DebugC DebugC { get ; set; }
    private ElectricalStripValues electricalStripValues;
    private ElectricalStripController electricalStripController;

    float r = Constants.electricalStripBaseSize.x;
    float s = Constants.electricalStripSeparatorSize;
    [SerializeField] private bool resetBoard = false;
    public bool ResetBoard { get{ return resetBoard; } set{ resetBoard = value; } }
    [SerializeField] private GameObject socketPrefab;
    [SerializeField] [Range(1, 10)] private int width = 1;
    [SerializeField] [Range(1, 10)] private int height = 2;
    [SerializeField] List<SocketsRow> cachedSocketsActiveGrid = new List<SocketsRow>();
    [SerializeField] List<SocketsRow> socketsActiveGrid = new List<SocketsRow>();
    [SerializeField] private GameObject backgroundVisual;
    [SerializeField] private GameObject socketsParent;
    [SerializeField] private GameObject powerSwitch;

    public List<SocketsRow> SocketsActiveGrid {get{return socketsActiveGrid;} set{socketsActiveGrid = value;}}

    private bool temporarilyModifiable = false;
    private RectTransform rectTransform;
    private Mouse   mouse = Mouse.current;
    private Vector2 cachedMousePosition;

    public bool TemporarilyModifiable {get{return temporarilyModifiable;} set{temporarilyModifiable = value;}}
    //private List<Transform> sockets = new List<Transform>();
    //private int socketCount = 2;
    RectTransform rectangularTransform;
    Vector2 size;

    // Start is called before the first frame update
    void Start() {
        DebugC = DebugC.Get();
        electricalStripValues     = GetComponent<ElectricalStripValues>();
        electricalStripController = electricalStripValues.ElectricalStripController;
        rectangularTransform = backgroundVisual.GetComponent<RectTransform>();
        size = rectangularTransform.sizeDelta;
    }

    // Update is called once per frame
    void Update() {
        //DebugC.Log(Mouse.current.position.value);
        rectangularTransform = backgroundVisual.GetComponent<RectTransform>();
        Vector2 newSize = new Vector2((Constants.electricalStripBaseSize.x + Constants.electricalStripSeparatorSize)*width  + Constants.electricalStripSeparatorSize, 
                                      (Constants.electricalStripBaseSize.y + Constants.electricalStripSeparatorSize)*height + 2*Constants.electricalStripSeparatorSize + Constants.powerSwitchBaseSize.y);
        if(size != newSize || resetBoard) {
            RenewSockets();
            MovePowerSwitch();
        }
    }


    public void OnBeginDrag(PointerEventData eventData) {
        if(!TemporarilyModifiable) { return; }
        cachedMousePosition = mouse.position.value;
    }
    public void OnDrag(PointerEventData eventData) {
        if(!TemporarilyModifiable) { return; }
        if(math.abs(cachedMousePosition.x - mouse.position.value.x) > Constants.electricalStripBaseSize.x) {
            if(mouse.position.value.x > cachedMousePosition.x) { ModifySize(Directions.Right); }
            else                                               { ModifySize(Directions.Left); }
            cachedMousePosition.x = mouse.position.value.x;
        }
        else if(math.abs(cachedMousePosition.y - mouse.position.value.y) > Constants.electricalStripBaseSize.y) {
            if(mouse.position.value.y > cachedMousePosition.y) { ModifySize(Directions.Up); }
            else                                               { ModifySize(Directions.Down); }
            cachedMousePosition.y = mouse.position.value.y;
        }
    }


    private void ModifySize(Directions direction) {
        switch(direction) {
            case Directions.Up:
                if(height < 6) { height++; }
                break;
            case Directions.Down:
                if(height > 1) { height--; }
                break;
            case Directions.Left:
                if(width > 1) { width--; }
                break;
            case Directions.Right:
                if(width < 10) { width++; }
                break;
        }
    }


    public void RenewSockets() {
        rectangularTransform = backgroundVisual.GetComponent<RectTransform>();
        Vector2 newSize = new Vector2((Constants.electricalStripBaseSize.x + Constants.electricalStripSeparatorSize)*width  + Constants.electricalStripSeparatorSize, 
                                      (Constants.electricalStripBaseSize.y + Constants.electricalStripSeparatorSize)*height + 2*Constants.electricalStripSeparatorSize + Constants.powerSwitchBaseSize.y);
        rectangularTransform.sizeDelta = newSize;
        //socketCount = width * height;
        size = rectangularTransform.sizeDelta;
        resetBoard = false;

        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        //DebugC.Log(size);
        Vector2 topLeft = new Vector2(center.x - size.x/2, center.y + size.y/2);
        electricalStripController.SocketsGrid = new Transform[height, width];

        RenewSocketsActiveGrid();


        for(int i=0; i<socketsParent.transform.childCount; i++) {
            socketsParent.transform.GetChild(i).gameObject.SetActive(false);
        }

        int childCount = socketsParent.transform.childCount;
        int index = 0;
        //DebugC.Log("width: "+electricalStripController.SocketsGrid.GetLength(0));
        //DebugC.Log("height: "+electricalStripController.SocketsGrid.GetLength(1));
        for(int i=0; i<electricalStripController.SocketsGrid.GetLength(0); i++) {
            for(int j=0; j<electricalStripController.SocketsGrid.GetLength(1); j++) {
                if(index >= childCount) {
                    GameObject newSocket = Instantiate(socketPrefab, socketsParent.transform);
                    newSocket.name = "Socket"+(index+1);
                    electricalStripController.SocketsGrid[i, j] = newSocket.transform;
                    newSocket.GetComponent<SocketManager>().id = new Index2D(i, j);
                }
                else {
                    electricalStripController.SocketsGrid[i, j] = socketsParent.transform.GetChild(index);
                    foreach(Transform child in electricalStripController.SocketsGrid[i, j].GetComponent<SocketManager>().ChildrenTransforms) {
                        child.gameObject.SetActive(true);
                    }
                    electricalStripController.SocketsGrid[i, j].GetComponent<SocketManager>().IsActive = true;
                    electricalStripController.SocketsGrid[i, j].GetComponent<SocketManager>().id = new Index2D(i, j);
                }
                if(socketsActiveGrid[i].Row[j] == false) { 
                    Debug.Log($"Socket At ({i}, {j}) is inactive."); 
                    foreach(Transform child in electricalStripController.SocketsGrid[i, j].GetComponent<SocketManager>().ChildrenTransforms) {
                        child.gameObject.SetActive(false);
                    }
                    electricalStripController.SocketsGrid[i, j].gameObject.SetActive(true);
                    electricalStripController.SocketsGrid[i, j].GetComponent<SocketManager>().IsActive = false;
                    electricalStripController.SocketsGrid[i, j].GetComponent<SocketManager>().id = new Index2D(i, j);
                }
                index++;
            }
        }
        DebugC.LogArray2D("", electricalStripController.SocketsGrid);
        //Utilities.LogList<Transform>("sockets before: ", sockets);
        //sockets.RemoveAt(0);
        //Utilities.LogList<Transform>("sockets after: ", sockets);
        for(int i=1; i<=height; i++) {
            for(int j=1; j<=width; j++) {
                //sockets[count].position = topLeft;
                //DebugC.Log(r*(i - 0.5f));
                //DebugC.Log(r*(i - 0.5f) + s*i);
                electricalStripController.SocketsGrid[i-1,j-1].position = new Vector2(topLeft.x + r*(j - 0.5f) + s*j,
                                                                                      topLeft.y - r*(i - 0.5f) - s*i - (Constants.powerSwitchBaseSize.y + s));
            }
        }
        //DebugC.Log($"sockets count: {sockets.Count()}, actual count: {socketCount}");
        //sockets.RemoveRange(socketCount,sockets.Count()-socketCount);

        //DebugC.Log($"sockets count: {sockets.Count()}, actual count: {socketCount}");
    }

    private void MovePowerSwitch() {
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        Vector2 topLeft = new Vector2(center.x - size.x/2, center.y + size.y/2);
        powerSwitch.transform.position = new Vector2(center.x, topLeft.y - Constants.powerSwitchBaseSize.y/2 - s);
    }

    private void RenewSocketsActiveGrid() {
        if(socketsActiveGrid == null) {
            socketsActiveGrid.Clear();
            for(int i=0; i<height; i++) {
                socketsActiveGrid.Add(new SocketsRow(width));
            }
        }
        else {
            List<SocketsRow> temp = new List<SocketsRow>();
            for(int i=0; i<socketsActiveGrid.Count; i++) { temp.Add(new SocketsRow(socketsActiveGrid[i], width)); }

            socketsActiveGrid.Clear();
            for(int i=0; i<height; i++) {
                if(i < temp.Count) { socketsActiveGrid.Add(new SocketsRow(temp[i], width)); }
                else { socketsActiveGrid.Add(new SocketsRow(width)); }
            }
        }
        cachedSocketsActiveGrid = socketsActiveGrid;
    }
}
