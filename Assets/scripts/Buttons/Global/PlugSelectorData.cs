using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlugSelectorData : MonoBehaviour {
    [HideInInspector] public PlugSelectorController plugSelectorController;
    [HideInInspector] public ControlsData           controlsData;
    [HideInInspector] public ControlsController     controlsController;
    [HideInInspector] public InputActionReference   mouseScrollAction;

    //buttons will be automatically generated
    [SerializeField]  public GameObject obstaclesParent;
    [SerializeField]  public GameObject plugsParent;
    [SerializeField]  public GameObject background;
    [SerializeField]  public GameObject buttonPrefab;
    [SerializeField] 
    public List<PlugSelectorAtributes>  allSelectablePlugs = new List<PlugSelectorAtributes>();
    [SerializeField]  public Vector2     startingOffset     = new Vector2(30, 15); //from bottom left
    [SerializeField]  public Vector2     buttonSize         = new Vector2(220, 220);
    [SerializeField]  public float       offset             = 30f;
    [SerializeField]  public float       scrollSpeed        = 10f;
    [HideInInspector] public bool        isHoveringOver     = false;
    [HideInInspector] public IEnumerator scrollCoroutine;


    void Awake() {
        plugSelectorController = Utilities.TryGetComponent<PlugSelectorController>(gameObject);
        controlsData = FindObjectOfType<ControlsData>();
        controlsController = FindObjectOfType<ControlsController>();
        mouseScrollAction = controlsData.mouseScrollAction;
    }
}
