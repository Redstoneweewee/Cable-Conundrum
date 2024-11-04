using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsData : MonoBehaviour {
    public IntersectionController      intersectionController;
    public ElectricalStripData       electricalStripData;
    public ElectricalStripController electricalStripController;

    public InputActionReference exitAction;
    public InputActionReference jointAction;
    public InputActionReference obstaclesAction;

    //These are only available for the editor, not the actual game
    public InputActionReference plugSelectorAction;
    public InputActionReference electricalStripAction;
    public InputActionReference mouseScrollAction;
    public InputActionReference deleteAction;

    public GameObject plugSelectorCanvas;

    [HideInInspector] public bool masterJointsEnabled    = false;
    [HideInInspector] public bool obstaclesModifiable    = false;
    [HideInInspector] public bool plugSelectorEnabled    = false;
    [HideInInspector] public bool electricalStripEnabled = false;



    void OnEnable() {
        exitAction.action.Enable();
        jointAction.action.Enable();
        obstaclesAction.action.Enable();
        plugSelectorAction.action.Enable();
        electricalStripAction.action.Enable();
        mouseScrollAction.action.Enable();
        deleteAction.action.Enable();
    }

    void Awake() {
        intersectionController      = FindObjectOfType<IntersectionController>();
        electricalStripData       = FindObjectOfType<ElectricalStripData>();
        electricalStripController = electricalStripData.electricalStripController;
    }
}
