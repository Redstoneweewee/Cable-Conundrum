using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class JointsController : MonoBehaviour, IDebugC  {
    public DebugC DebugC { get ; set; }
    private ControlsManager controlsManager;
    private JointsOpacityController jointsOpacityController;
    private ElectricalStripValues electricalStripValues;
    [SerializeField] private GameObject jointPrefab;
    private bool cachedJointsEnabled = false;
    [SerializeField] private bool jointsEnabled = false;
    public bool JointsEnabled { get{return jointsEnabled;} set{jointsEnabled = value;} }
    private Vector2 cachedScreenSize;
    private Transform[,] jointsGrid;
    public Transform[,] JointsGrid { get{return jointsGrid;} private set{jointsGrid = value;} }
    
    [SerializeField] Material jointMaterial;
    float r = Constants.electricalStripBaseSize.x;
    float s = Constants.electricalStripSeparatorSize;

    // Start is called before the first frame update
    void Start() {
        DebugC = DebugC.Get();
        controlsManager = FindObjectOfType<ControlsManager>();
        jointsOpacityController = GetComponent<JointsOpacityController>();
        electricalStripValues = Utilities.GetElectricalStripValues();
        cachedScreenSize = new Vector2(Screen.width, Screen.height);
        jointMaterial.color = Constants.jointColor;
        StartCoroutine(startDelayed());
    }

    private IEnumerator startDelayed() {
        yield return new WaitForEndOfFrame();
        RenewJoints();
        jointsEnabled = controlsManager.MasterJointsEnabled;
    }

    // Update is called once per frame
    void Update() {

        //Debug.Log("isFirstOpacity: "+isFirstOpacity);
        if((jointsEnabled == true && cachedScreenSize.x != Screen.width || cachedScreenSize.y != Screen.height) || cachedJointsEnabled != jointsEnabled) {
            RenewJoints();
            cachedScreenSize = new Vector2(Screen.width, Screen.height);
        }
        if(cachedJointsEnabled != jointsEnabled && jointsEnabled == false) {
            cachedJointsEnabled = jointsEnabled;
        }
        else if(cachedJointsEnabled != jointsEnabled && jointsEnabled == true) {
            if(jointsOpacityController.OpacityCoroutine == null) {
                jointsOpacityController.OpacityCoroutine = jointsOpacityController.ModifyJointsOpacity();
                StartCoroutine(jointsOpacityController.OpacityCoroutine);
                Debug.Log("restarted");
            }
            else {
                jointsOpacityController.IsFirstLoop = false;
            }
            //jointsOpacityController.tfd = 0;
            jointsOpacityController.PreviousIsFirstOpacity = jointsOpacityController.IsFirstOpacity;
            jointsOpacityController.IsFirstOpacity = true;
            jointsOpacityController.CalculateDValues();
            cachedJointsEnabled = jointsEnabled;
        }
    }

    private void RenewJoints() {
        float step = Constants.jointDistance;
        if(electricalStripValues.ElectricalStripController.SocketsGrid == null) { electricalStripValues.ElectricalStripSizeController.RenewSockets(); }
        if(electricalStripValues.ElectricalStripController.SocketsGrid[0,0] == null) { electricalStripValues.ElectricalStripSizeController.RenewSockets(); }
        
        Vector2 topLeft = electricalStripValues.ElectricalStripController.SocketsGrid[0,0].position;
        while(topLeft.x > step)                 { topLeft.x -= step; }
        while(topLeft.y < Screen.height - step) { topLeft.y += step; }
        topLeft.x -= step; topLeft.y += step;

        int width  = (int)((Screen.width - topLeft.x)/step) + 2;
        int height = (int)(topLeft.y/step) + 2;
        jointsGrid = new Transform[height, width];
        

        //for(int i=0; i<transform.childCount; i++) {
        //    transform.GetChild(i).gameObject.SetActive(false);
        //}

        int childCount = transform.childCount;
        int index = 0;
        //Debug.Log("width: "+electricalStripController.SocketsGrid.GetLength(0));
        //Debug.Log("height: "+electricalStripController.SocketsGrid.GetLength(1));
        for(int i=0; i<jointsGrid.GetLength(0); i++) {
            for(int j=0; j<jointsGrid.GetLength(1); j++) {
                if(index >= childCount) {
                    GameObject newJoint = Instantiate(jointPrefab, transform);
                    newJoint.name = "Joint"+(index+1);
                    jointsGrid[i, j] = newJoint.transform;
                }
                else {
                    jointsGrid[i, j] = transform.GetChild(index);
                    //jointsGrid[i, j].gameObject.SetActive(jointsEnabled);
                }
                index++;
            }
        }
        DebugC.LogArray2D("", jointsGrid);
        //Utilities.LogList<Transform>("sockets before: ", sockets);
        //sockets.RemoveAt(0);
        //Utilities.LogList<Transform>("sockets after: ", sockets);
        for(int i=0; i<height; i++) {
            for(int j=0; j<width; j++) {
                //sockets[count].position = topLeft;
                //DebugC.Log(r*(i - 0.5f));
                //DebugC.Log(r*(i - 0.5f) + s*i);
                jointsGrid[i,j].position = new Vector2(topLeft.x + (j*step),
                                                       topLeft.y - (i*step));
            }
        }
    }

}
