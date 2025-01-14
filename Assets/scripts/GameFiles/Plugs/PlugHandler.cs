using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlugHandler : ScriptInitializerBase, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler {
    private PlugAttributes A;

    public override IEnumerator Initialize() {
        A = Utilities.TryGetComponent<PlugAttributes>(gameObject);
        yield return null;
    }

    public void OnBeginDrag(PointerEventData eventData) {
    }

    public void OnDrag(PointerEventData eventData) {
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

    public void OnPointerClick(PointerEventData eventData) {
        DebugC.Instance.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(A.isObstacle && !A.obstacleAttributes.temporarilyModifiable) { return; }
        if(eventData.pointerCurrentRaycast.gameObject == A.plugVisual) { return; }
        TryModifyCables();
        if(ControlsData.Instance.masterJointsEnabled) { JointsData.Instance.jointsEnabled = true; }
    }
    /// <summary>
    /// OnMouseDown() only works when:
    /// 1. The Main Camera is seeing the entire UI Canvas (must be resized with canvas size)
    /// 2. Collider2D(s) are added to this gameobject
    /// </summary>
    void OnMouseDown() {
        if(A.isObstacle && !A.obstacleAttributes.temporarilyModifiable) { return; }
        A.transform.SetAsLastSibling();
        StartDrag();
    }

    public void OnPointerEnter(PointerEventData eventData) {
    }

    public void OnPointerExit(PointerEventData eventData) {
    }

    public void OnPointerUp(PointerEventData eventData) {
        if(A.isObstacle && !A.obstacleAttributes.temporarilyModifiable) { return; }
        if(eventData.pointerCurrentRaycast.gameObject == A.plugVisual) { return; }
        A.isDragging = false;
        A.isModifyingCables = false;
        if(!ControlsData.Instance.masterJointsEnabled) { JointsData.Instance.jointsEnabled = false; }
    }
    void OnMouseUp() {
        if(A.isObstacle && !A.obstacleAttributes.temporarilyModifiable) { return; }
        Utilities.TryGetComponent<RectTransform>(A.plugVisual).localScale = new Vector3(1f, 1f, 1f);
        A.isDragging = false;
        A.isModifyingCables = false;
        if(!ControlsData.Instance.masterJointsEnabled) { JointsData.Instance.jointsEnabled = false; }

    }

    private void StartDrag() {
        A.offset = new Vector2(transform.position.x, transform.position.y) - ControlsController.Instance.GetPointerPosition();
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
        if(A.isDragging) { A.targetPosition = ControlsController.Instance.GetPointerPosition() + A.offset; }
        
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
                transform.position = firstNearestSocket.position - new Vector3(A.localSnapPositions[0].x*LevelResizeGlobal.Instance.finalScale, A.localSnapPositions[0].y*LevelResizeGlobal.Instance.finalScale, 0);
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
        GridsController.Instance.RenewPlugsGrid();
        A.cableHandler.InitializeCableGrid();
        GridsController.Instance.RenewAllCablesGrid();
        if(!A.isObstacle) {   
            Utilities.SetCablesOpacity(A.cableParentAttributes.gameObject, 1f);
        }
        IntersectionController.Instance.TestForCableIntersection();

        foreach(SoundsAttributes soundsAttribute in SoundsData.Instance.soundEffects) {
            if(soundsAttribute.soundType == SoundTypes.PlugSnapEnter) {
                SoundPlayer.PlaySound(soundsAttribute, SoundsData.Instance.soundVolume);
            }
        }
    }

    public void PlugOut() {
        A.isPluggedIn = false;
        GridsController.Instance.RenewPlugsGrid();
        A.cableHandler.ResetCableGrid();
        GridsController.Instance.RenewAllCablesGrid();
        if(!A.isObstacle) { 
            Utilities.SetCablesOpacity(A.cableParentAttributes.gameObject, Constants.cableOpacity);
        }
        IntersectionController.Instance.TestForCableIntersection();
        //A.intersectionController.ClearAllCableIntersections();
        ////A.intersectionController.ClearCableIntersections(A);

        foreach(SoundsAttributes soundsAttribute in SoundsData.Instance.soundEffects) {
            if(soundsAttribute.soundType == SoundTypes.PlugSnapExit) {
                SoundPlayer.PlaySound(soundsAttribute, SoundsData.Instance.soundVolume);
            }
        }
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
        int[,] allCablesGrid = GridsData.Instance.allCablesGrid;
        int[,] plugsGrid = GridsData.Instance.plugsGrid;
        bool[,] allObstaclesGrid = GridsData.Instance.allObstaclesGrid;
        
        Transform[,] socketsGrid = GridsData.Instance.socketsGrid;

        Transform nearestSocket = socketsGrid[0, 0]; //This is just here to make the error go away. Doesn't actually do anything.

        for(int a=0; a<A.localSnapPositions.Count; a++) {
            Vector3 position = A.cachedPlugPositionDynamic + A.localSnapPositions[a]*LevelResizeGlobal.Instance.finalScale;
            Index2D jointsGridIndex   = Utilities.CalculateJointsGridIndex(position);
            Index2D socketsGridIndex = Utilities.CalculateSocketsGridIndex(position);
            
            if(a == 0) { nearestSocket = socketsGrid[socketsGridIndex.x,socketsGridIndex.y]; }

            float distance = (socketsGrid[socketsGridIndex.x, socketsGridIndex.y].position - position).magnitude;
            //tests if the plug is plugged into the socket and has no been moved too far away. 
            //If the statement is true, the plug does not get evicted yet and keep testing for each LocalSnapPosition.
            if(A.isPluggedIn && distance <= LevelResizeGlobal.Instance.plugLockingDistance) { continue; }
            //tests if:
            //1. The socket is inactive
            //2. There are cables blocking the socket
            //3. There is already a plug on the socket
            //4. The socket is too far away (distance > Constants.plugLockingDistance)
            //If any of those conditions are true, then the plug cannot be plugged in, therefore return null.
            else if(distance > LevelResizeGlobal.Instance.plugLockingDistance ||
                !Utilities.TryGetComponent<SocketAttributes>(socketsGrid[socketsGridIndex.x,socketsGridIndex.y].gameObject).isActive || 
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
        DebugC.Instance.Log($"InitialCreateDrag: {A.name}");
        A.offset = new Vector2(transform.position.x, transform.position.y) - ControlsController.Instance.GetPointerPosition();
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
        if(A.isDragging) { A.targetPosition = ControlsController.Instance.GetPointerPosition() + A.offset; }
        DebugC.Instance.Log($"isDragging: {A.isDragging}, mouse: {Input.GetMouseButton(0)}");
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
                transform.position = firstNearestSocket.position - new Vector3(A.localSnapPositions[0].x*LevelResizeGlobal.Instance.finalScale, A.localSnapPositions[0].y*LevelResizeGlobal.Instance.finalScale, 0);
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