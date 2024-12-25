using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Can be altered in Edit Mode
//A script that can alter all grids.
//[ExecuteInEditMode]
public class GridsModifier : Singleton<GridsModifier> {
    [SerializeField] [Range(1, 10)] public int width = 1;
    [SerializeField] [Range(1, 10)] public int height = 2;
    [SerializeField] public bool renewGrids;
    [SerializeField] public bool testDotsActive;
    [SerializeField] public GameObject testingDot;
    [SerializeField] public GameObject testingDotCanvas;
    private bool cachedtestDotsActive;
    private int cachedWidth;
    private int cachedHeight;


    public override void OnAwake() {
        GridsSkeleton.Instance.Initialize();
    }
    void Update() {
        if(renewGrids || testDotsActive != cachedtestDotsActive || width != cachedWidth || height != cachedHeight) {
            GridsSkeleton.Instance.Initialize();
            FindFirstObjectByType<GridsController>().Initialize();
            renewGrids = false;
            cachedtestDotsActive = testDotsActive;
            cachedWidth = width;
            cachedHeight = height;
        }
    }

    public void DeleteAllTestDots() {
        GameObject[] allTestDots = GameObject.FindGameObjectsWithTag("TestDot");
        foreach(GameObject testDot in allTestDots) {
            DestroyImmediate(testDot);
        }
    }
}
