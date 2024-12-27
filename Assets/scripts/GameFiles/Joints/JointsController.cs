using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class JointsController : Singleton<JointsController> {
    private JointsData D;

    public override void OnAwake() {
        D = JointsData.Instance;
    }
    
    void Start() {
        StartCoroutine(startDelayed());
    }

    private IEnumerator startDelayed() {
        yield return new WaitForEndOfFrame();
        //RenewJoints();
        D.jointsEnabled = ControlsData.Instance.masterJointsEnabled;
    }

    // Update is called once per frame
    void Update() {

        //Debug.Log("isFirstOpacity: "+isFirstOpacity);
        if((D.jointsEnabled == true && D.cachedScreenSize.x != Screen.width || D.cachedScreenSize.y != Screen.height) || D.cachedJointsEnabled != D.jointsEnabled) {
            //RenewJoints();
            D.cachedScreenSize = new Vector2(Screen.width, Screen.height);
        }
        if(D.cachedJointsEnabled != D.jointsEnabled && D.jointsEnabled == false) {
            D.cachedJointsEnabled = D.jointsEnabled;
        }
        else if(D.cachedJointsEnabled != D.jointsEnabled && D.jointsEnabled == true) {
            if(JointsOpacityGlobal.Instance.OpacityCoroutine == null) {
                JointsOpacityGlobal.Instance.OpacityCoroutine = JointsOpacityGlobal.Instance.ModifyJointsOpacity();
                StartCoroutine(JointsOpacityGlobal.Instance.OpacityCoroutine);
            }
            else {
                JointsOpacityGlobal.Instance.IsFirstLoop = false;
            }
            //jointsOpacityController.tfd = 0;
            JointsOpacityGlobal.Instance.PreviousIsFirstOpacity = JointsOpacityGlobal.Instance.IsFirstOpacity;
            JointsOpacityGlobal.Instance.IsFirstOpacity = true;
            JointsOpacityGlobal.Instance.CalculateDValues();
            D.cachedJointsEnabled = D.jointsEnabled;
        }
    }

    /*
    private void RenewJoints() {
        float step = Constants.jointDistance;
        if(D.electricalStripData.socketsGrid == null) { D.electricalStripData.electricalStripSizeController.RenewSockets(); }
        if(D.electricalStripData.socketsGrid[0,0] == null) { D.electricalStripData.electricalStripSizeController.RenewSockets(); }
        
        Vector2 topLeft = D.electricalStripData.socketsGrid[0,0].position;
        while(topLeft.x > step)                 { topLeft.x -= step; }
        while(topLeft.y < Screen.height - step) { topLeft.y += step; }
        topLeft.x -= step; topLeft.y += step;

        int width  = (int)((Screen.width - topLeft.x)/step) + 2;
        int height = (int)(topLeft.y/step) + 2;
        D.jointsGrid = new Transform[height, width];
        

        //for(int i=0; i<transform.childCount; i++) {
        //    transform.GetChild(i).gameObject.SetActive(false);
        //}

        int childCount = transform.childCount;
        int index = 0;
        //Debug.Log("width: "+electricalStripController.SocketsGrid.GetLength(0));
        //Debug.Log("height: "+electricalStripController.SocketsGrid.GetLength(1));
        for(int i=0; i<D.jointsGrid.GetLength(0); i++) {
            for(int j=0; j<D.jointsGrid.GetLength(1); j++) {
                if(index >= childCount) {
                    GameObject newJoint = Instantiate(D.jointPrefab, transform);
                    newJoint.name = "Joint"+(index+1);
                    D.jointsGrid[i, j] = newJoint.transform;
                }
                else {
                    D.jointsGrid[i, j] = transform.GetChild(index);
                    //jointsGrid[i, j].gameObject.SetActive(jointsEnabled);
                }
                index++;
            }
        }
        D.debugC.LogArray2D("", D.jointsGrid);
        //Utilities.LogList<Transform>("sockets before: ", sockets);
        //sockets.RemoveAt(0);
        //Utilities.LogList<Transform>("sockets after: ", sockets);
        for(int i=0; i<height; i++) {
            for(int j=0; j<width; j++) {
                //sockets[count].position = topLeft;
                //DebugC.Log(r*(i - 0.5f));
                //DebugC.Log(r*(i - 0.5f) + s*i);
                D.jointsGrid[i,j].position = new Vector2(topLeft.x + (j*step),
                                                       topLeft.y - (i*step));
            }
        }
    }
    */

}
