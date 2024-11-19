using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControlsController : MonoBehaviour {
    ControlsData D;
    AdminToggles adminToggles;
    
    void Awake() {
        D = FindObjectOfType<ControlsData>();
    }
    void Start() {
        SubscribeToActionStart(D.exitAction, OnExit);
        SubscribeToActionStart(D.jointAction, OnJointsToggle);
        SubscribeToActionStart(D.obstaclesAction, OnObstaclesToggle);
        SubscribeToActionStart(D.plugSelectorAction, OnPlugSelectorToggle);
        SubscribeToActionStart(D.electricalStripAction, OnElectricalStripToggle);
        SubscribeToActionStart(D.deleteAction, OnTryDeletePlug);
        SubscribeToActionStart(D.deleteAction, OnTryDeleteObstacle);
        ChangeEditorMode();
    }

    void Update() {
        ChangeEditorMode();
    }

    private void ChangeEditorMode() {
        adminToggles = FindObjectOfType<AdminToggles>();
        if(!adminToggles.cachedEditorMode && adminToggles.editorMode) {
            D.obstaclesAction.action.Enable();
            D.plugSelectorAction.action.Enable();
            D.electricalStripAction.action.Enable();
            D.deleteAction.action.Enable();
            adminToggles.cachedEditorMode = adminToggles.editorMode;
        }
        else if(adminToggles.cachedEditorMode && !adminToggles.editorMode) {
            D.obstaclesAction.action.Disable();
            D.plugSelectorAction.action.Disable();
            D.electricalStripAction.action.Disable();
            D.deleteAction.action.Disable();
            adminToggles.cachedEditorMode = adminToggles.editorMode;
        }
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






    private void OnExit(InputAction.CallbackContext context) {
        Debug.Log("bring up the pause menu");
    }

    private bool IsNotInALevel() {
        if(SceneManager.GetActiveScene().buildIndex < Constants.firstLevelBuidIndex) { return true; }
        return false;
    }

    private void OnJointsToggle(InputAction.CallbackContext context) {
        if(IsNotInALevel()) { return; }
        D.masterJointsEnabled = !D.masterJointsEnabled;
        FindObjectOfType<JointsData>().jointsEnabled = D.masterJointsEnabled;
        Debug.Log("chaged masterJointsEnabled: "+D.masterJointsEnabled);
    }

    private void OnObstaclesToggle(InputAction.CallbackContext context) {
        if(IsNotInALevel()) { return; }
        D.obstaclesModifiable = !D.obstaclesModifiable;
        ObstacleAttributes[] obstacleAttributes = FindObjectsOfType<ObstacleAttributes>();
        if(D.obstaclesModifiable) {
            foreach(ObstacleAttributes obstacleAttribute in obstacleAttributes) {
                obstacleAttribute.obstacleHandler.SetOpacity(0.8f);
                obstacleAttribute.temporarilyModifiable = true;
            }   
        }
        else {
            foreach(ObstacleAttributes obstacleAttribute in obstacleAttributes) {
                obstacleAttribute.obstacleHandler.SetOpacity(1);
                obstacleAttribute.temporarilyModifiable = false;
            }   
        }
        Debug.Log("chaged obstaclesModifiable: "+D.obstaclesModifiable);
    }

    private void OnPlugSelectorToggle(InputAction.CallbackContext context) {
        if(IsNotInALevel()) { return; }
        D.plugSelectorEnabled = !D.plugSelectorEnabled;

        if(D.plugSelectorEnabled) { D.plugSelectorCanvas.transform.GetChild(0).gameObject.SetActive(true); }
        else { 
            if(Utilities.TryGetComponent<PlugSelectorData>(D.plugSelectorCanvas.transform.GetChild(0).gameObject).scrollCoroutine != null) {
                StopCoroutine(Utilities.TryGetComponent<PlugSelectorData>(D.plugSelectorCanvas.transform.GetChild(0).gameObject).scrollCoroutine);
            }
            Utilities.TryGetComponent<PlugSelectorData>(D.plugSelectorCanvas.transform.GetChild(0).gameObject).scrollCoroutine = null;
            D.plugSelectorCanvas.transform.GetChild(0).gameObject.SetActive(false); 
        }
    }

    private void OnElectricalStripToggle(InputAction.CallbackContext context) {
        if(IsNotInALevel()) { return; }
        D.electricalStripEnabled = !D.electricalStripEnabled;
        GameObject electricalStrip = FindObjectOfType<ElectricalStripController>().gameObject;
        
        if(D.electricalStripEnabled) {
            Utilities.TryGetComponent<CanvasGroup>(electricalStrip).alpha = 0.8f;
            D.electricalStripData.temporarilyModifiable = true;
        }
        else {
            Utilities.TryGetComponent<CanvasGroup>(electricalStrip).alpha = 1f;
            D.electricalStripData.temporarilyModifiable = false;
        }
    
    }

    
    private void OnTryDeletePlug(InputAction.CallbackContext context) {
        if(IsNotInALevel()) { return; }
        PlugAttributes[] allPlugAttributes = FindObjectsOfType<PlugAttributes>();
        foreach(PlugAttributes plugAttribute in allPlugAttributes) {
            if(plugAttribute.isDragging) { 
                Destroy(plugAttribute.gameObject); 
                StartCoroutine(RenewGrids());
            }
        }
    }

    private void OnTryDeleteObstacle(InputAction.CallbackContext context) {
        if(IsNotInALevel()) { return; }
        ObstacleAttributes[] allObstacleAttributes = FindObjectsOfType<ObstacleAttributes>();
        foreach(ObstacleAttributes obstacleAttribute in allObstacleAttributes) {
            if(obstacleAttribute.temporarilyModifiable && obstacleAttribute.isDragging) { 
                Destroy(obstacleAttribute.gameObject);
                StartCoroutine(RenewGrids());
            }
        }
    }

    private IEnumerator RenewGrids() {
        yield return new WaitForEndOfFrame();
        D.gridsController.RenewAllObstaclesGrid();
        D.gridsController.RenewAllCablesGrid();
        D.intersectionController.TestForCableIntersection();
    }

}
