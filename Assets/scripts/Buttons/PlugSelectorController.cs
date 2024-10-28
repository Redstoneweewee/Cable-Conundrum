using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum PlugSelectorType { Plug, PermaPlug, Table }

[Serializable]
public class PlugSelectorAtributes {
    private GameObject buttonGameObject;
    [SerializeField] private GameObject plugPrefab;
    [SerializeField] private GameObject plugImage;
    [SerializeField] private PlugSelectorType type;
    private Button   button;
    public GameObject       ButtonGameObject {get{return buttonGameObject;}         set{buttonGameObject = value;}}
    public GameObject       PlugPrefab       {get{return plugPrefab;}       private set{plugPrefab       = value;}}
    public GameObject       PlugImage        {get{return plugImage;}        private set{plugImage        = value;}}
    public PlugSelectorType Type             {get{return type;}             private set{type             = value;}}
    public Button           Button           {get{return button;}                   set{button           = value;}}
}


public class PlugSelectorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    //buttons will be automatically generated
    [SerializeField] private GameObject obstaclesParent;
    [SerializeField] private GameObject plugsParent;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private List<PlugSelectorAtributes> allSelectablePlugs = new List<PlugSelectorAtributes>();
    [SerializeField] private Vector2 startingOffset = new Vector2(30, 15); //from bottom left
    [SerializeField] private Vector2 buttonSize = new Vector2(220, 220);
    [SerializeField] private float offset = 30f;
    [SerializeField] private float scrollSpeed = 10f;

    private ControlsController controlsController;
    private ControlsManager controlsManager;
    private InputActionReference mouseScrollAction;

    private bool isHoveringOver = false;
    private IEnumerator scrollCoroutine;

    
    public IEnumerator ScrollCoroutine {get{return scrollCoroutine;} set{scrollCoroutine = value;}}

    // Start is called before the first frame update
    void Start() {
        controlsController = FindObjectOfType<ControlsController>();
        controlsManager = FindObjectOfType<ControlsManager>();
        mouseScrollAction = controlsController.MouseScrollAction;
        InitializeButtons();
        //SubscribeButtons();
    }

    // Update is called once per frame
    void Update() {
    }


    private void InitializeButtons() {
        Vector3 buttonPosition = new Vector3(buttonSize.x/2+startingOffset.x, buttonSize.y/2+startingOffset.y, 0);
        float backgroundLength = startingOffset.x;
        int index = 0;
        foreach(PlugSelectorAtributes attribute in allSelectablePlugs) {
            GameObject buttonGameObject = Instantiate(buttonPrefab, transform);
            buttonGameObject.transform.position = buttonPosition;

            attribute.ButtonGameObject = buttonGameObject;
            Button button = buttonGameObject.GetComponentInChildren<Button>();
            attribute.Button = button;

            Instantiate(attribute.PlugImage, buttonGameObject.transform);

            button.GetComponent<ButtonId>().id = index;

            buttonPosition = new Vector3(buttonPosition.x + buttonSize.x + offset, buttonPosition.y, buttonPosition.z);
            backgroundLength += buttonSize.x + offset;
            if(backgroundLength > background.GetComponent<RectTransform>().sizeDelta.x) {
                background.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundLength, background.GetComponent<RectTransform>().sizeDelta.y);
            }
            index++;
        }
    }

    //private void SubscribeButtons() {
    //    foreach(PlugSelectorAtributes attribute in allSelectablePlugs) {
    //        attribute.Button.onClick.AddListener(OnClickPlugSelectorButton);
    //    }
    //}



    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("Mouse Enter");
        isHoveringOver = true;
        if(scrollCoroutine == null) {
            scrollCoroutine = TestForScroll();
            StartCoroutine(scrollCoroutine);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log("Mouse Exit");
        isHoveringOver = false;
    }


    private IEnumerator TestForScroll() {
        yield return new WaitForEndOfFrame();

        Vector2 mouseScroll = controlsController.GetActionInputValue<Vector2>(mouseScrollAction);
        RectTransform rectTransform = background.GetComponent<RectTransform>();
        //Debug.Log($"background.transform.position.x: {rectTransform.position.x}");
        //Debug.Log($"+++: {background.transform.position.x + rectTransform.sizeDelta.x/2}");
        //Debug.Log($"---: {background.transform.position.x - rectTransform.sizeDelta.x/2}");
        //Scrolling up = go back/right
        if(mouseScroll.y > 0) { 
            if(rectTransform.position.x < 0) {
                transform.position = new Vector3(transform.position.x+scrollSpeed, transform.position.y, transform.position.z);
            }
            if(rectTransform.position.x > 0) {
                float delta = rectTransform.position.x;
                transform.position = new Vector3(transform.position.x-delta, transform.position.y, transform.position.z);
            }
        }
        else if(mouseScroll.y < 0) {
            if(rectTransform.position.x + rectTransform.sizeDelta.x > Screen.width) {
                transform.position = new Vector3(transform.position.x-scrollSpeed, transform.position.y, transform.position.z);
            }
            if(rectTransform.position.x + rectTransform.sizeDelta.x < Screen.width) {
                float delta = Screen.width - (rectTransform.position.x + rectTransform.sizeDelta.x);
                transform.position = new Vector3(transform.position.x+delta, transform.position.y, transform.position.z);
            }
        }

        //Scrolling down = go forward/left

        if(isHoveringOver) {
            scrollCoroutine = TestForScroll();
            StartCoroutine(scrollCoroutine);
        }
        else {
            if(scrollCoroutine != null) { StopCoroutine(scrollCoroutine); }
            scrollCoroutine = null;
        }
    }




    public void OnClickPlugSelectorButton(int buttonId) {
        //int buttonId = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonId>().id;
        Debug.Log("A button was pressed.");
        Debug.Log($"buttonId: {buttonId}");
        PlugSelectorAtributes attribute = allSelectablePlugs[buttonId];
        if(attribute.Type == PlugSelectorType.Plug) {
            GameObject plug = Instantiate(attribute.PlugPrefab, plugsParent.transform);
            plug.transform.position = Mouse.current.position.value;
            plug.GetComponent<PlugInteractions>().InitialCreateDrag();
        }
        else if(attribute.Type == PlugSelectorType.PermaPlug) {
            GameObject plug = Instantiate(attribute.PlugPrefab, plugsParent.transform);
            plug.transform.position = Mouse.current.position.value;
            plug.GetComponent<Obstacle>().TemporarilyModifiable = controlsManager.ObstaclesModifiable;
            plug.GetComponent<PlugInteractions>().InitialCreateDrag();
        }
        else if(attribute.Type == PlugSelectorType.Table) {
            GameObject table = Instantiate(attribute.PlugPrefab, obstaclesParent.transform);
            table.GetComponent<Obstacle>().TemporarilyModifiable = controlsManager.ObstaclesModifiable;
            if(controlsManager.ObstaclesModifiable) { table.GetComponent<Obstacle>().SetOpacity(0.8f); }
        }
    }





}
