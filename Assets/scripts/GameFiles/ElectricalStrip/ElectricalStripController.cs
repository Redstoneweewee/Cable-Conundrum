using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//[ExecuteInEditMode]
public class ElectricalStripController : Singleton<ElectricalStripController>, IDragHandler, IBeginDragHandler {
    private ElectricalStripData D;

    // Start is called before the first frame update
    public override void OnAwake() {
        D = ElectricalStripData.Instance;
    }

    void Update() {
        D.rectangularTransform = Utilities.TryGetComponent<RectTransform>(D.backgroundVisual);
    }


    public void OnBeginDrag(PointerEventData eventData) {
        if(!D.temporarilyModifiable) { return; }
        D.cachedMousePosition = ControlsController.Instance.GetPointerPosition();
    }
    public void OnDrag(PointerEventData eventData) {
        if(!D.temporarilyModifiable) { return; }
        if(math.abs(D.cachedMousePosition.x - ControlsController.Instance.GetPointerPosition().x) > LevelResizeGlobal.Instance.electricalStripBaseSize.x) {
            if(ControlsController.Instance.GetPointerPosition().x > D.cachedMousePosition.x) { ModifySize(Directions.Right); }
            else                                               { ModifySize(Directions.Left); }
            D.cachedMousePosition.x = ControlsController.Instance.GetPointerPosition().x;
        }
        else if(math.abs(D.cachedMousePosition.y - ControlsController.Instance.GetPointerPosition().y) > LevelResizeGlobal.Instance.electricalStripBaseSize.y) {
            if(ControlsController.Instance.GetPointerPosition().y > D.cachedMousePosition.y) { ModifySize(Directions.Up); }
            else                                               { ModifySize(Directions.Down); }
            D.cachedMousePosition.y = ControlsController.Instance.GetPointerPosition().y;
        }
    }

    private void CreateBackgroundVisual() {

    }

    private void ModifySize(Directions direction) {
        switch(direction) {
            case Directions.Up:
                if(GridsModifier.Instance.height < 6) { GridsModifier.Instance.height++; }
                break;
            case Directions.Down:
                if(GridsModifier.Instance.height > 1) { GridsModifier.Instance.height--; }
                break;
            case Directions.Left:
                if(GridsModifier.Instance.width > 1) { GridsModifier.Instance.width--; }
                break;
            case Directions.Right:
                if(GridsModifier.Instance.width < 10) { GridsModifier.Instance.width++; }
                break;
        }
    }

    public void ModifyBackgroundVisual() {
        Vector2 newSize = new Vector2((LevelResizeGlobal.staticElectricalStripBaseSize.x + LevelResizeGlobal.staticElectricalStripSeparatorDistance)*GridsModifier.Instance.width  + LevelResizeGlobal.staticElectricalStripSeparatorDistance, 
                                      (LevelResizeGlobal.staticElectricalStripBaseSize.y + LevelResizeGlobal.staticElectricalStripSeparatorDistance)*GridsModifier.Instance.height + 2*LevelResizeGlobal.staticElectricalStripSeparatorDistance + LevelResizeGlobal.staticPowerSwitchBaseSize.y);
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        D.rectangularTransform.sizeDelta = newSize;
        D.rectangularTransform.position = new Vector3(center.x, center.y+(LevelResizeGlobal.Instance.electricalStripSeparatorDistance + LevelResizeGlobal.Instance.powerSwitchBaseSize.y)/2, 0);
        MovePowerSwitch();
    }

    private void MovePowerSwitch() {
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        Vector2 topLeft = new Vector2(D.rectangularTransform.position.x - (D.rectangularTransform.sizeDelta.x/2*LevelResizeGlobal.Instance.finalScale), D.rectangularTransform.position.y + (D.rectangularTransform.sizeDelta.y/2*LevelResizeGlobal.Instance.finalScale));
        D.powerSwitch.transform.position = new Vector2(center.x, topLeft.y - (LevelResizeGlobal.Instance.powerSwitchBaseSize.y/2 + LevelResizeGlobal.Instance.electricalStripSeparatorDistance));
    }
}
