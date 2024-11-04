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
    
    // Start is called before the first frame update
    void Start() {
        D = FindObjectOfType<ControlsData>();

        SubscribeToActionStart(D.exitAction, OnExit);
        SubscribeToActionStart(D.jointAction, OnJointsToggle);
        SubscribeToActionStart(D.obstaclesAction, OnObstaclesToggle);
        SubscribeToActionStart(D.plugSelectorAction, OnPlugSelectorToggle);
        SubscribeToActionStart(D.electricalStripAction, OnElectricalStripToggle);
        SubscribeToActionStart(D.deleteAction, OnTryDeletePlug);
        SubscribeToActionStart(D.deleteAction, OnTryDeleteObstacle);
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

    private bool IsNotInLevel() {
        if(SceneManager.GetActiveScene().buildIndex == 0) { return true; }
        //if(SceneManager.GetActiveScene().buildIndex == 1) { return true; }
        return false;
    }

    private void OnJointsToggle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        D.masterJointsEnabled = !D.masterJointsEnabled;
        FindObjectOfType<JointsController>().JointsEnabled = D.masterJointsEnabled;
        Debug.Log("chaged masterJointsEnabled: "+D.masterJointsEnabled);
    }

    private void OnObstaclesToggle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        D.obstaclesModifiable = !D.obstaclesModifiable;
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        if(D.obstaclesModifiable) {
            foreach(Obstacle obstacle in obstacles) {
                obstacle.SetOpacity(0.8f);
                obstacle.TemporarilyModifiable = true;
            }   
        }
        else {
            foreach(Obstacle obstacle in obstacles) {
                obstacle.SetOpacity(1);
                obstacle.TemporarilyModifiable = false;
            }   
        }
        Debug.Log("chaged obstaclesModifiable: "+D.obstaclesModifiable);
    }

    private void OnPlugSelectorToggle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        D.plugSelectorEnabled = !D.plugSelectorEnabled;

        if(D.plugSelectorEnabled) { D.plugSelectorCanvas.transform.GetChild(0).gameObject.SetActive(true); }
        else { 
            if(D.plugSelectorCanvas.transform.GetChild(0).GetComponent<PlugSelectorData>().scrollCoroutine != null) {
                StopCoroutine(D.plugSelectorCanvas.transform.GetChild(0).GetComponent<PlugSelectorData>().scrollCoroutine);
            }
            D.plugSelectorCanvas.transform.GetChild(0).GetComponent<PlugSelectorData>().scrollCoroutine = null;
            D.plugSelectorCanvas.transform.GetChild(0).gameObject.SetActive(false); 
        }
    }

    private void OnElectricalStripToggle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        D.electricalStripEnabled = !D.electricalStripEnabled;
        GameObject electricalStrip = FindObjectOfType<ElectricalStripController>().gameObject;
        
        if(D.electricalStripEnabled) {
            electricalStrip.GetComponent<CanvasGroup>().alpha = 0.8f;
            D.electricalStripData.temporarilyModifiable = true;
        }
        else {
            electricalStrip.GetComponent<CanvasGroup>().alpha = 1f;
            D.electricalStripData.temporarilyModifiable = false;
        }
    
    }

    
    private void OnTryDeletePlug(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        PlugInteractions[] allPlugInteractions = FindObjectsOfType<PlugInteractions>();
        foreach(PlugInteractions plugInteractions in allPlugInteractions) {
            if(plugInteractions.IsDragging) { 
                Destroy(plugInteractions.gameObject); 
                StartCoroutine(RenewGrids());
            }
        }
    }

    private void OnTryDeleteObstacle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        Obstacle[] allObstacles = FindObjectsOfType<Obstacle>();
        foreach(Obstacle obstacle in allObstacles) {
            if(obstacle.TemporarilyModifiable && obstacle.IsDragging) { 
                Destroy(obstacle.gameObject);
                StartCoroutine(RenewGrids());
            }
        }
    }

    private IEnumerator RenewGrids() {
        yield return new WaitForEndOfFrame();
        D.intersectionController.RenewAllObstaclesGrid();
        D.electricalStripController.RenewAllCableGrids();
        D.intersectionController.TestForCableIntersection();
    }

}
