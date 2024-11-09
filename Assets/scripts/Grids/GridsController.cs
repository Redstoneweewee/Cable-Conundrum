using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsController : MonoBehaviour {
    private GridsData D;
    private GridsSizeInitializer I;


    void Awake() {
        D = GetComponent<GridsData>();
        I = GetComponent<GridsSizeInitializer>();
        I.Initialize();
        Debug.Log("jointsSkeletonGrid: "+I.jointsSkeletonGrid);
        Debug.Log("socketsSkeletonGrid: "+I.socketsSkeletonGrid);
    }

    // Update is called once per frame
    void Update() {

    }
}
