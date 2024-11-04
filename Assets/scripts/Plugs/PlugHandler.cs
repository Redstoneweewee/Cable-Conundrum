using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlugHandler : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler {
    private PlugAttributes A;

    void Awake() {
        A = Utilities.TryGetComponent<PlugAttributes>(gameObject);
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
        if(A.isObstacle && !A.obstacleAttributes.temporarilyModifiable) { return; }
        A.transform.SetAsLastSibling();
        if(eventData.pointerCurrentRaycast.gameObject != A.plugVisual) { 
            TryModifyCables();
            if(A.controlsData.masterJointsEnabled) { A.jointsData.jointsEnabled = true; }
            return;
        }
        StartDrag();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log("Mouse Enter");
    }

    public void OnPointerExit(PointerEventData eventData) {
        //Debug.Log("Mouse Exit");
    }

    public void OnPointerUp(PointerEventData eventData) {
        if(A.isObstacle && !A.obstacleAttributes.temporarilyModifiable) { return; }
        Utilities.TryGetComponent<RectTransform>(A.plugVisual).localScale = new Vector3(1f, 1f, 1f);
        TryModifyCables();
        //electricalStripController.RenewAllCableGrids();
        A.isDragging = false;
        A.isModifyingCables = false;
        if(!A.controlsData.masterJointsEnabled) { A.jointsData.jointsEnabled = false; }
    }

    private void StartDrag() {
        A.offset = new Vector2(transform.position.x, transform.position.y) - A.mouse.position.value;
        //transform.localScale = new Vector3(1f, 1f, 1f);
        //transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
        Utilities.TryGetComponent<RectTransform>(A.plugVisual).localScale = new Vector3(0.97f, 0.97f, 0.97f);
        A.cachedPlugPositionDynamic = transform.position;
        A.isDragging = true;
        A.dragCoroutine = DragPlug();
        StartCoroutine(A.dragCoroutine);
    }

    private IEnumerator DragPlug() {
        yield return new WaitForEndOfFrame();
        if(A.isDragging) { A.targetPosition = A.mouse.position.value + A.offset; }
        
        if(!Utilities.IsApproximate(A.targetPosition, A.cachedPlugPositionDynamic - A.offset, 0.01f)) {
            A.cachedPlugPositionDynamic = Vector2.Lerp(A.cachedPlugPositionDynamic, A.targetPosition, Constants.plugInterpolation);
            Transform firstNearestSocket = PlugIntoSocketTest();
            //Debug.Log("firstNearestSocket: "+firstNearestSocket);

            if(firstNearestSocket == null) {
                transform.position = A.cachedPlugPositionDynamic;
                if(A.isPluggedIn == true) {
                    PlugOut();
                }
            }
            else {
                transform.position = firstNearestSocket.position - new Vector3(A.localSnapPositions[0].x, A.localSnapPositions[0].y, 0);
                if(A.isPluggedIn == false) {
                    PlugIn();
                }
            }
        }


        if(A.isDragging || !Utilities.IsApproximate(A.targetPosition, A.cachedPlugPositionDynamic, 0.01f)) {
            A.dragCoroutine = DragPlug();
            StartCoroutine(A.dragCoroutine);
        }
        else { 
            if(A.dragCoroutine != null) { StopCoroutine(A.dragCoroutine); }
            A.dragCoroutine = null;
            A.offset = Vector2.zero;
            A.cachedPlugPositionDynamic = Vector2.zero;
            A.targetPosition = Vector2.zero;
        }
    }

    public void PlugIn() {
        A.isPluggedIn = true;
        A.electricalStripController.RenewPlugsGrid();
        A.cableHandler.InitializeCableGrid();
        A.electricalStripController.RenewAllCableGrids();
        if(!A.isObstacle) { A.cableHandler.SetCablesOpacity(1f); }
        A.intersectionController.TestForCableIntersection();
    }

    public void PlugOut() {
        A.isPluggedIn = false;
        A.electricalStripController.RenewPlugsGrid();
        A.cableHandler.ResetCableGrid();
        A.electricalStripController.RenewAllCableGrids();
        if(!A.isObstacle) { A.cableHandler.SetCablesOpacity(Constants.cableOpacity); }
        //intersectionDetector.ClearAllCableIntersections();
        A.intersectionController.TestForCableIntersection();
    }

    private void TryModifyCables() {
        if(!A.isPluggedIn) { return; }
        A.isModifyingCables = true;
        A.cableHandler.InitializeCableGrid();
        A.cableHandler.InitializeCachedMouseGridIndex();
        A.modifyCableCoroutine = A.cableHandler.ModifyCablesOnInteract();
        StartCoroutine(A.modifyCableCoroutine);
    }


    private Transform PlugIntoSocketTest() {
        int[,] allCablesGrid = A.electricalStripData.allCablesGrid;
        int[,] plugsGrid = A.electricalStripData.plugsGrid;
        bool[,] allObstaclesGrid = A.intersectionData.allObstaclesGrid;
        
        Transform[,] socketsGrid = A.electricalStripData.socketsGrid;
        Transform[,] jointsGrid = A.jointsData.jointsGrid;
        float   subSocketLength  = Constants.jointDistance;
        float   subJointLength  = Constants.jointDistance/2;

        Transform nearestSocket = socketsGrid[0, 0]; //This is just here to make the error go away. Doesn't actually do anything.

        for(int a=0; a<A.localSnapPositions.Count; a++) {
            Vector3 position = A.cachedPlugPositionDynamic + A.localSnapPositions[a];

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
            if(A.isPluggedIn && distance <= Constants.plugLockingDistance) { continue; }
            //tests if:
            //1. The socket is inactive
            //2. There are cables blocking the socket
            //3. There is already a plug on the socket
            //4. The socket is too far away (distance > Constants.plugLockingDistance)
            //If any of those conditions are true, then the plug cannot be plugged in, therefore return null.
            else if(distance > Constants.plugLockingDistance ||
                !Utilities.TryGetComponent<SocketAttributes>(socketsGrid[socketGridIndex.x,socketGridIndex.y].gameObject).isActive || 
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
        Debug.Log($"InitialCreateDrag: {A}");
        A.offset = new Vector2(transform.position.x, transform.position.y) - A.mouse.position.value;
        //transform.localScale = new Vector3(1f, 1f, 1f);
        //transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
        Utilities.TryGetComponent<RectTransform>(A.plugVisual).localScale = new Vector3(0.97f, 0.97f, 0.97f);
        A.cachedPlugPositionDynamic = transform.position;
        A.isDragging = true;
        A.dragCoroutine = InitialCreateDragPlug();
        StartCoroutine(A.dragCoroutine);
    }

    private IEnumerator InitialCreateDragPlug() {
        yield return new WaitForEndOfFrame();
        if(!Input.GetMouseButton(0)) { A.isDragging = false; }
        if(A.isDragging) { A.targetPosition = A.mouse.position.value + A.offset; }
        Debug.Log($"isDragging: {A.isDragging}, mouse: {Input.GetMouseButton(0)}");
        if(!Utilities.IsApproximate(A.targetPosition, A.cachedPlugPositionDynamic - A.offset, 0.01f)) {
            A.cachedPlugPositionDynamic = Vector2.Lerp(A.cachedPlugPositionDynamic, A.targetPosition, Constants.plugInterpolation);
            Transform firstNearestSocket = PlugIntoSocketTest();
            //Debug.Log("firstNearestSocket: "+firstNearestSocket);

            if(firstNearestSocket == null) {
                transform.position = A.cachedPlugPositionDynamic;
                if(A.isPluggedIn == true) {
                    PlugOut();
                }
            }
            else {
                transform.position = firstNearestSocket.position - new Vector3(A.localSnapPositions[0].x, A.localSnapPositions[0].y, 0);
                if(A.isPluggedIn == false) {
                    PlugIn();
                }
            }
        }


        if(A.isDragging || !Utilities.IsApproximate(A.targetPosition, A.cachedPlugPositionDynamic, 0.01f)) {
            A.dragCoroutine = InitialCreateDragPlug();
            StartCoroutine(A.dragCoroutine);
        }
        else { 
            if(A.dragCoroutine != null) { StopCoroutine(A.dragCoroutine); }
            A.dragCoroutine = null;
            A.offset = Vector2.zero;
            A.cachedPlugPositionDynamic = Vector2.zero;
            A.targetPosition = Vector2.zero;
        }
    }
}