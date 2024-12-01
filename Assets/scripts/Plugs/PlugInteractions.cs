using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlugInteractions : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler {
    public DebugC DebugC {set; get;}
    private ControlsManager controlsManager;
    private ControlsController controlsController;
    private InputActionReference deleteAction;

    [SerializeField] private GameObject plugVisual;
    private Plug plug;
    private IntersectionDetector intersectionDetector;
    private CableGeneration cableGeneration;
    private IEnumerator dragCoroutine;
    private IEnumerator modifyCableCoroutine;
    private bool    isDragging         = false;
    private bool    isModifyingCables  = false;
    private Mouse   mouse              = Mouse.current;
    private JointsController jointsController;
    private ElectricalStripController electricalStripController;
            //Vector2 cachedPlugPositionActual  = Vector2.zero;
    private Vector2 cachedPlugPositionDynamic = Vector2.zero;
    private Vector2 targetPosition     = Vector2.zero;
    private Vector2 offset             = Vector2.zero;

    public bool        IsDragging           {get{return isDragging;          } set{isDragging           = value;}}
    public GameObject  PlugVisual           {get{return plugVisual;          } set{plugVisual           = value;}}
    public IEnumerator ModifyCableCoroutine {get{return modifyCableCoroutine;} set{modifyCableCoroutine = value;}}
    public bool        IsModifyingCables    {get{return isModifyingCables;   } set{isModifyingCables    = value;}}

    void Start() {
        DebugC = DebugC.Get();
        controlsManager = FindObjectOfType<ControlsManager>();
        intersectionDetector = FindObjectOfType<IntersectionDetector>();
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        plug = GetComponent<Plug>();
        jointsController = FindObjectOfType<JointsController>();
        cableGeneration = GetComponentInChildren<CableGeneration>();
        controlsController = FindObjectOfType<ControlsController>();
    }
    


    public void OnBeginDrag(PointerEventData eventData) {
        //Debug.Log("Drag Begin");
    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("Dragging");
    }

    public void OnEndDrag(PointerEventData eventData) {
        //Debug.Log("Drag Ended");
    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }
    public void OnPointerDown(PointerEventData eventData) {
        if(plug.IsObstacle && !plug.Obstacle.TemporarilyModifiable) { return; }
        if(eventData.pointerCurrentRaycast.gameObject == plugVisual) { return; }
        TryModifyCables();
        if(controlsManager.MasterJointsEnabled) { jointsController.JointsEnabled = true; }
    }

    void OnMouseDown() {
        if(plug.IsObstacle && !plug.Obstacle.TemporarilyModifiable) { return; }
        plug.transform.SetAsLastSibling();
        StartDrag();
    }
    void OnMouseUp() {
        if(plug.IsObstacle && !plug.Obstacle.TemporarilyModifiable) { return; }
        plugVisual.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        isModifyingCables = false;
        isDragging = false;
        if(!controlsManager.MasterJointsEnabled) { jointsController.JointsEnabled = false; }
    }


    public void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log("Mouse Enter");
    }

    public void OnPointerExit(PointerEventData eventData) {
        //Debug.Log("Mouse Exit");
    }

    public void OnPointerUp(PointerEventData eventData) {
        if(plug.IsObstacle && !plug.Obstacle.TemporarilyModifiable) { return; }
        if(eventData.pointerCurrentRaycast.gameObject == plugVisual) { return; }
        //TryModifyCables();
        isModifyingCables = false;
        isDragging = false;
        if(!controlsManager.MasterJointsEnabled) { jointsController.JointsEnabled = false; }
    }

    private void StartDrag() {
        offset = new Vector2(transform.position.x, transform.position.y) - mouse.position.value;
        //transform.localScale = new Vector3(1f, 1f, 1f);
        //transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
        plugVisual.GetComponent<RectTransform>().localScale = new Vector3(0.97f, 0.97f, 0.97f);
        cachedPlugPositionDynamic = transform.position;
        isDragging = true;
        dragCoroutine = DragPlug();
        StartCoroutine(dragCoroutine);
    }

    private IEnumerator DragPlug() {
        yield return new WaitForEndOfFrame();
        if(isDragging) { targetPosition = mouse.position.value + offset; }
        
        if(!Utilities.IsApproximate(targetPosition, cachedPlugPositionDynamic - offset, 0.01f)) {
            cachedPlugPositionDynamic = Vector2.Lerp(cachedPlugPositionDynamic, targetPosition, Constants.plugInterpolation);
            Transform firstNearestSocket = PlugIntoSocketTest();
            //Debug.Log("firstNearestSocket: "+firstNearestSocket);

            if(firstNearestSocket == null) {
                transform.position = cachedPlugPositionDynamic;
                if(plug.IsPluggedIn == true) {
                    PlugOut();
                }
            }
            else {
                transform.position = firstNearestSocket.position - new Vector3(plug.LocalSnapPositions[0].x, plug.LocalSnapPositions[0].y, 0);
                if(plug.IsPluggedIn == false) {
                    PlugIn();
                }
            }
        }


        if(isDragging || !Utilities.IsApproximate(targetPosition, cachedPlugPositionDynamic, 0.01f)) {
            dragCoroutine = DragPlug();
            StartCoroutine(dragCoroutine);
        }
        else { 
            if(dragCoroutine != null) { StopCoroutine(dragCoroutine); }
            dragCoroutine = null;
            offset = Vector2.zero;
            cachedPlugPositionDynamic = Vector2.zero;
            targetPosition = Vector2.zero;
        }
    }

    public void PlugIn() {
        plug.IsPluggedIn = true;
        electricalStripController.RenewPlugsGrid();
        cableGeneration.InitializeCableGrid();
        electricalStripController.RenewAllCableGrids();
        if(!plug.IsObstacle) { cableGeneration.SetCablesOpacity(1f); }
        intersectionDetector.TestForCableIntersection();
    }

    public void PlugOut() {
        plug.IsPluggedIn = false;
        electricalStripController.RenewPlugsGrid();
        cableGeneration.ResetCableGrid();
        electricalStripController.RenewAllCableGrids();
        if(!plug.IsObstacle) { cableGeneration.SetCablesOpacity(Constants.cableOpacity); }
        //intersectionDetector.ClearAllCableIntersections();
        intersectionDetector.TestForCableIntersection();
    }

    private void TryModifyCables() {
        if(!plug.IsPluggedIn) { return; }
        isModifyingCables = true;
        cableGeneration.InitializeCableGrid();
        cableGeneration.InitializeCachedMouseGridIndex();
        modifyCableCoroutine = cableGeneration.ModifyCablesOnInteract();
        StartCoroutine(modifyCableCoroutine);
    }


    private Transform PlugIntoSocketTest() {
        int[,] allCablesGrid = electricalStripController.AllCablesGrid;
        int[,] plugsGrid = electricalStripController.PlugsGrid;
        bool[,] allObstaclesGrid = intersectionDetector.AllObstaclesGrid;
        
        Transform[,] socketsGrid = electricalStripController.SocketsGrid;
        Transform[,] jointsGrid = jointsController.JointsGrid;
        float   subSocketLength  = Constants.jointDistance;
        float   subJointLength  = Constants.jointDistance/2;

        Transform nearestSocket = socketsGrid[0, 0]; //This is just here to make the error go away. Doesn't actually do anything.

        for(int a=0; a<plug.LocalSnapPositions.Count; a++) {
            Vector3 position = cachedPlugPositionDynamic + plug.LocalSnapPositions[a];

            Vector2 distanceFromTopLeftJoint = new Vector2(position.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - position.y);
            Index2D jointsGridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
            jointsGridIndex          = new Index2D(Math.Clamp(jointsGridIndex.y, 0, jointsGrid.GetLength(0)-1), Math.Clamp(jointsGridIndex.x, 0, jointsGrid.GetLength(1)-1));

            Vector2 distanceFromTopLeftSocket = new Vector2(position.x - socketsGrid[0,0].position.x, socketsGrid[0,0].position.y - position.y);
            Index2D socketGridIndex = new Index2D(((int)(distanceFromTopLeftSocket.x/subSocketLength)+1)/2, ((int)(distanceFromTopLeftSocket.y/subSocketLength)+1)/2);
            socketGridIndex         = new Index2D(Math.Clamp(socketGridIndex.y, 0, socketsGrid.GetLength(0)-1), Math.Clamp(socketGridIndex.x, 0, socketsGrid.GetLength(1)-1));
            
            if(a == 0) { nearestSocket = socketsGrid[socketGridIndex.x,socketGridIndex.y]; }

            float distance = (socketsGrid[socketGridIndex.x, socketGridIndex.y].position - position).magnitude;
            //tests if the plug is plugged into the socket and has no been moved too far away. 
            //If the statement is true, the plug does not get evicted yet and keep testing for each LocalSnapPosition.
            if(plug.isPluggedIn && distance <= Constants.plugLockingDistance) { continue; }
            //tests if:
            //1. The socket is inactive
            //2. There are cables blocking the socket
            //3. There is already a plug on the socket
            //4. The socket is too far away (distance > Constants.plugLockingDistance)
            //If any of those conditions are true, then the plug cannot be plugged in, therefore return null.
            else if(distance > Constants.plugLockingDistance ||
                !socketsGrid[socketGridIndex.x,socketGridIndex.y].GetComponent<SocketManager>().IsActive || 
                allCablesGrid[jointsGridIndex.x, jointsGridIndex.y] != 0 || 
                allObstaclesGrid[jointsGridIndex.x,jointsGridIndex.y] ||
                plugsGrid[jointsGridIndex.x,jointsGridIndex.y] > 0) 
                { return null; }
            //otherwise, keep testing for each LocalSnapPosition
        }
        //If all tests are passed, return the Transform of the socket related to the first LocalSnapPosition
        return nearestSocket;
    }


    public void InitialCreateDrag() {
        offset = new Vector2(transform.position.x, transform.position.y) - mouse.position.value;
        //transform.localScale = new Vector3(1f, 1f, 1f);
        //transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
        plugVisual.GetComponent<RectTransform>().localScale = new Vector3(0.97f, 0.97f, 0.97f);
        cachedPlugPositionDynamic = transform.position;
        isDragging = true;
        dragCoroutine = InitialCreateDragPlug();
        StartCoroutine(dragCoroutine);
    }

    private IEnumerator InitialCreateDragPlug() {
        yield return new WaitForEndOfFrame();
        if(!Input.GetMouseButton(0)) { isDragging = false; }
        if(isDragging) { targetPosition = mouse.position.value + offset; }
        Debug.Log($"isDragging: {isDragging}, mouse: {Input.GetMouseButton(0)}");
        if(!Utilities.IsApproximate(targetPosition, cachedPlugPositionDynamic - offset, 0.01f)) {
            cachedPlugPositionDynamic = Vector2.Lerp(cachedPlugPositionDynamic, targetPosition, Constants.plugInterpolation);
            Transform firstNearestSocket = PlugIntoSocketTest();
            //Debug.Log("firstNearestSocket: "+firstNearestSocket);

            if(firstNearestSocket == null) {
                transform.position = cachedPlugPositionDynamic;
                if(plug.IsPluggedIn == true) {
                    PlugOut();
                }
            }
            else {
                transform.position = firstNearestSocket.position - new Vector3(plug.LocalSnapPositions[0].x, plug.LocalSnapPositions[0].y, 0);
                if(plug.IsPluggedIn == false) {
                    PlugIn();
                }
            }
        }


        if(isDragging || !Utilities.IsApproximate(targetPosition, cachedPlugPositionDynamic, 0.01f)) {
            dragCoroutine = InitialCreateDragPlug();
            StartCoroutine(dragCoroutine);
        }
        else { 
            if(dragCoroutine != null) { StopCoroutine(dragCoroutine); }
            dragCoroutine = null;
            offset = Vector2.zero;
            cachedPlugPositionDynamic = Vector2.zero;
            targetPosition = Vector2.zero;
        }
    }
}