using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlugSelectorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private PlugSelectorData D;

    void Awake() {
        D = Utilities.TryGetComponent<PlugSelectorData>(gameObject);
        InitializeButtons();
    }

    // |----------------------------------------------------------------------------------|
    // |----------------------------------------------------------------------------------|
    // |----- Save and LoadData does NOT work with PlugSelector dragged in plugs!!!! -----|
    // |----------------------------------------------------------------------------------|
    // |----------------------------------------------------------------------------------|

    private void InitializeButtons() {
        Vector3 buttonPosition = new Vector3(D.buttonSize.x/2+D.startingOffset.x, D.buttonSize.y/2+D.startingOffset.y, 0);
        float backgroundLength = D.startingOffset.x;
        int index = 0;
        foreach(PlugSelectorAtributes attribute in D.allSelectablePlugs) {
            GameObject buttonGameObject = Instantiate(D.buttonPrefab, transform);
            buttonGameObject.transform.position = buttonPosition;

            attribute.ButtonGameObject = buttonGameObject;
            Button button = Utilities.TryGetComponentInChildren<Button>(buttonGameObject);
            attribute.Button = button;

            Instantiate(attribute.PlugImage, buttonGameObject.transform);

            Utilities.TryGetComponent<PlugSelectorButtonsLocal>(button.gameObject).id = index;

            buttonPosition = new Vector3(buttonPosition.x + D.buttonSize.x + D.offset, buttonPosition.y, buttonPosition.z);
            backgroundLength += D.buttonSize.x + D.offset;
            if(backgroundLength > Utilities.TryGetComponent<RectTransform>(D.background).sizeDelta.x) {
                Utilities.TryGetComponent<RectTransform>(D.background).sizeDelta = new Vector2(backgroundLength, Utilities.TryGetComponent<RectTransform>(D.background).sizeDelta.y);
            }
            index++;
        }
    }


    public void OnPointerEnter(PointerEventData eventData) {
        D.isHoveringOver = true;
        if(D.scrollCoroutine == null) {
            D.scrollCoroutine = TestForScroll();
            StartCoroutine(D.scrollCoroutine);
        }
    }


    public void OnPointerExit(PointerEventData eventData) {
        D.isHoveringOver = false;
    }


    private IEnumerator TestForScroll() {
        yield return new WaitForEndOfFrame();

        Vector2 mouseScroll = D.controlsController.GetActionInputValue<Vector2>(D.mouseScrollAction);
        RectTransform rectTransform = Utilities.TryGetComponent<RectTransform>(D.background);
        if(mouseScroll.y > 0) { 
            if(rectTransform.position.x < 0) {
                transform.position = new Vector3(transform.position.x+D.scrollSpeed, transform.position.y, transform.position.z);
            }
            if(rectTransform.position.x > 0) {
                float delta = rectTransform.position.x;
                transform.position = new Vector3(transform.position.x-delta, transform.position.y, transform.position.z);
            }
        }
        else if(mouseScroll.y < 0) {
            if(rectTransform.position.x + rectTransform.sizeDelta.x > Screen.width) {
                transform.position = new Vector3(transform.position.x-D.scrollSpeed, transform.position.y, transform.position.z);
            }
            if(rectTransform.position.x + rectTransform.sizeDelta.x < Screen.width) {
                float delta = Screen.width - (rectTransform.position.x + rectTransform.sizeDelta.x);
                transform.position = new Vector3(transform.position.x+delta, transform.position.y, transform.position.z);
            }
        }

        if(D.isHoveringOver) {
            D.scrollCoroutine = TestForScroll();
            StartCoroutine(D.scrollCoroutine);
        }
        else {
            if(D.scrollCoroutine != null) { StopCoroutine(D.scrollCoroutine); }
            D.scrollCoroutine = null;
        }
    }


    public void OnClickPlugSelectorButton(int buttonId) {
        DebugC.Get().Log($"A button was pressed. ButtonId: {buttonId}");
        PlugSelectorAtributes attribute = D.allSelectablePlugs[buttonId];
        if(attribute.Type == PlugSelectorTypes.Plug) {
            GameObject plug = Instantiate(attribute.PlugPrefab, D.plugsParent.transform);
            plug.transform.position = Mouse.current.position.value;
            Utilities.TryGetComponent<PlugHandler>(plug).InitialCreateDrag();
        }
        else if(attribute.Type == PlugSelectorTypes.PermaPlug) {
            GameObject plug = Instantiate(attribute.PlugPrefab, D.plugsParent.transform);
            plug.transform.position = Mouse.current.position.value;
            Utilities.TryGetComponent<ObstacleAttributes>(plug).temporarilyModifiable = D.controlsData.obstaclesModifiable;
            Utilities.TryGetComponent<PlugHandler>(plug).InitialCreateDrag();
        }
        else if(attribute.Type == PlugSelectorTypes.Table) {
            GameObject table = Instantiate(attribute.PlugPrefab, D.obstaclesParent.transform);
            Utilities.TryGetComponent<ObstacleAttributes>(table).temporarilyModifiable = D.controlsData.obstaclesModifiable;
            if(D.controlsData.obstaclesModifiable) { Utilities.TryGetComponent<ObstacleHandler>(table).SetOpacity(0.8f); }
        }
        //D.levelInitializerGlobal.AddPlugs();
    }
}
