using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControlsManager : MonoBehaviour {
    ControlsController controlsController;
    IntersectionDetector intersectionDetector;
    ElectricalStripController electricalStripController;
    ElectricalStripSizeController electricalStripSizeController;
    private InputActionReference exitAction;
    private InputActionReference jointAction;
    private InputActionReference obstaclesAction;
    private InputActionReference plugSelectorAction;
    private InputActionReference electricalStripAction;
    private InputActionReference deleteAction;
    private GameObject plugSelectorCanvas;
    private bool masterJointsEnabled = false;
    private bool obstaclesModifiable = false;
    private bool plugSelectorEnabled = false;
    private bool electricalStripEnabled = false;
    public bool MasterJointsEnabled    {get{return masterJointsEnabled;}    set{masterJointsEnabled    = value;}}
    public bool ObstaclesModifiable    {get{return obstaclesModifiable;}    set{obstaclesModifiable    = value;}}
    public bool PlugSelectorEnabled    {get{return plugSelectorEnabled;}    set{plugSelectorEnabled    = value;}}
    public bool ElectricalStripEnabled {get{return electricalStripEnabled;} set{electricalStripEnabled = value;}}

    // Start is called before the first frame update
    void Start() {
        controlsController = FindObjectOfType<ControlsController>();
        intersectionDetector = FindObjectOfType<IntersectionDetector>();
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        electricalStripSizeController = FindObjectOfType<ElectricalStripSizeController>();
        exitAction = controlsController.ExitAction;
        jointAction = controlsController.JointAction;
        obstaclesAction = controlsController.ObstaclesAction;
        plugSelectorAction = controlsController.PlugSelectorAction;
        electricalStripAction = controlsController.ElectricalStripAction;
        deleteAction = controlsController.DeleteAction;

        controlsController.SubscribeToActionStart(exitAction, OnExit);
        controlsController.SubscribeToActionStart(jointAction, OnJointsToggle);
        controlsController.SubscribeToActionStart(obstaclesAction, OnObstaclesToggle);
        controlsController.SubscribeToActionStart(plugSelectorAction, OnPlugSelectorToggle);
        controlsController.SubscribeToActionStart(electricalStripAction, OnElectricalStripToggle);
        controlsController.SubscribeToActionStart(deleteAction, OnTryDeletePlug);
        controlsController.SubscribeToActionStart(deleteAction, OnTryDeleteObstacle);

        plugSelectorCanvas = controlsController.PlugSelectorCanvas;
    }

    // Update is called once per frame
    //void Update() {
//
    //}

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
        masterJointsEnabled = !masterJointsEnabled;
        FindObjectOfType<JointsController>().JointsEnabled = masterJointsEnabled;
        Debug.Log("chaged masterJointsEnabled: "+masterJointsEnabled);
    }

    private void OnObstaclesToggle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        obstaclesModifiable = !obstaclesModifiable;
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        if(obstaclesModifiable) {
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
        Debug.Log("chaged obstaclesModifiable: "+obstaclesModifiable);
    }

    private void OnPlugSelectorToggle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        plugSelectorEnabled = !plugSelectorEnabled;

        if(plugSelectorEnabled) { plugSelectorCanvas.transform.GetChild(0).gameObject.SetActive(true); }
        else { 
            if(plugSelectorCanvas.transform.GetChild(0).GetComponent<PlugSelectorController>().ScrollCoroutine != null) {
                StopCoroutine(plugSelectorCanvas.transform.GetChild(0).GetComponent<PlugSelectorController>().ScrollCoroutine);
            }
            plugSelectorCanvas.transform.GetChild(0).GetComponent<PlugSelectorController>().ScrollCoroutine = null;
            plugSelectorCanvas.transform.GetChild(0).gameObject.SetActive(false); 
        }
    }

    private void OnElectricalStripToggle(InputAction.CallbackContext context) {
        if(IsNotInLevel()) { return; }
        electricalStripEnabled = !electricalStripEnabled;
        GameObject electricalStrip = FindObjectOfType<ElectricalStripController>().gameObject;
        
        if(electricalStripEnabled) {
            electricalStrip.GetComponent<CanvasGroup>().alpha = 0.8f;
            electricalStripSizeController.TemporarilyModifiable = true;
        }
        else {
            electricalStrip.GetComponent<CanvasGroup>().alpha = 1f;
            electricalStripSizeController.TemporarilyModifiable = false;
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
        intersectionDetector.RenewAllObstaclesGrid();
        electricalStripController.RenewAllCableGrids();
        intersectionDetector.TestForCableIntersection();
    }

}
