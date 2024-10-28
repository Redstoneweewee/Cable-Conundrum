using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsController : MonoBehaviour {
    [SerializeField] private InputActionReference exitAction;
    [SerializeField] private InputActionReference jointAction;
    [SerializeField] private InputActionReference obstaclesAction;
    //These are only available for the editor, not the actual game
    [SerializeField] private InputActionReference plugSelectorAction;
    [SerializeField] private InputActionReference electricalStripAction;
    [SerializeField] private InputActionReference mouseScrollAction;
    [SerializeField] private InputActionReference deleteAction;

    [SerializeField] private GameObject plugSelectorCanvas;

    
    public InputActionReference ExitAction            { get{ return exitAction;            } set{ exitAction            = value; } }
    public InputActionReference JointAction           { get{ return jointAction;           } set{ jointAction           = value; } }
    public InputActionReference ObstaclesAction       { get{ return obstaclesAction;       } set{ obstaclesAction       = value; } }
    public InputActionReference PlugSelectorAction    { get{ return plugSelectorAction;    } set{ plugSelectorAction    = value; } }
    public InputActionReference ElectricalStripAction { get{ return electricalStripAction; } set{ electricalStripAction = value; } }
    public InputActionReference MouseScrollAction     { get{ return mouseScrollAction;     } set{ mouseScrollAction     = value; } }
    public InputActionReference DeleteAction          { get{ return deleteAction;          } set{ deleteAction          = value; } }

    public GameObject           PlugSelectorCanvas   { get{ return plugSelectorCanvas;   } set{ plugSelectorCanvas   = value; } }





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
    }

    /// <summary>
    /// Subscribes a function for when Action's button is initially pressed down. 
    /// Only call this once as an initializer.
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="Function"></param>
    public void SubscribeToActionStart(InputActionReference Action, Action<InputAction.CallbackContext> Function) {
        Action.action.started += Function;
    }

    /// <summary>
    /// Subscribes a function for when Action's button is initially pressed down. 
    /// Only call this once as an initializer.
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="Function"></param>
    public void SubscribeToActionExit(InputActionReference Action, Action<InputAction.CallbackContext> Function) {
        Action.action.canceled += Function;
    }

    /// <summary>
    /// Returns the current value of an action. Call this during Update().
    /// </summary>
    /// <typeparam name="VariableType">The type of the value to return (e.g., float, int).</typeparam>
    /// <param name="Action">The action whose current value is being retrieved.</param>
    /// <returns>The current value of the specified action.</returns>
    public VariableType GetActionInputValue<VariableType>(InputActionReference Action) {
        if(typeof(VariableType) == typeof(int)) { return (VariableType)(object)Action.action.ReadValue<int>(); }
        else if(typeof(VariableType) == typeof(float)) { return (VariableType)(object)Action.action.ReadValue<float>(); }
        else if(typeof(VariableType) == typeof(bool)) { return (VariableType)(object)Action.action.ReadValue<bool>(); }
        else if(typeof(VariableType) == typeof(Vector2)) { return (VariableType)(object)Action.action.ReadValue<Vector2>(); }
        else {
            Debug.LogError($"GetActionInputValue was called on {Action}, which has a variable type not defined in the GetActionInputValue function!");
            return (VariableType)(object)null;
        }
    }
}
