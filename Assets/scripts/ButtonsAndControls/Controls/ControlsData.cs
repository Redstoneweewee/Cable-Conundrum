using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsData : Singleton<ControlsData> {
    [SerializeField] public InputActionReference pointAction;

    [SerializeField] public InputActionReference exitAction;
    [SerializeField] public InputActionReference jointAction;
    [SerializeField] public InputActionReference obstaclesAction;

    //These are only available for the editor, not the actual game
    [SerializeField] public InputActionReference plugSelectorAction;
    [SerializeField] public InputActionReference electricalStripAction;
    [SerializeField] public InputActionReference mouseScrollAction;
    [SerializeField] public InputActionReference deleteAction;

    [SerializeField] public GameObject plugSelectorCanvas;

    [HideInInspector] public bool masterJointsEnabled    = false;
    [HideInInspector] public bool obstaclesModifiable    = false;
    [HideInInspector] public bool plugSelectorEnabled    = false;
    [HideInInspector] public bool electricalStripEnabled = false;

    public bool isUsed = true;


    public override IEnumerator Initialize() {
        //from EventSystem UI
        pointAction.action.Enable();

        //from custom controls
        exitAction.action.Enable();
        jointAction.action.Enable();
        obstaclesAction.action.Enable();
        plugSelectorAction.action.Enable();
        electricalStripAction.action.Enable();
        mouseScrollAction.action.Enable();
        deleteAction.action.Enable();
        yield return null;
    }
}
