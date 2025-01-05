using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Can be altered in Edit Mode
//A script that can alter all grids.
//[ExecuteInEditMode]
public class GridsModifier : Singleton<GridsModifier> {
    [SerializeField] [Range(1, 10)] public int width = 1;  //will de deprecated soon
    [SerializeField] [Range(1, 10)] public int height = 2;  //will de deprecated soon
    [SerializeField] public IntSize electricalStripSize = new(2, 1);
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
        if(renewGrids || testDotsActive != cachedtestDotsActive || electricalStripSize.width != cachedWidth || electricalStripSize.height != cachedHeight) {
            GridsSkeleton.Instance.Initialize();
            GridsController.Instance.Initialize();
            renewGrids = false;
            cachedtestDotsActive = testDotsActive;
            cachedWidth = (int)electricalStripSize.width;
            cachedHeight = (int)electricalStripSize.height;
        }
    }

    public void DeleteAllTestDots() {
        GameObject[] allTestDots = GameObject.FindGameObjectsWithTag("TestDot");
        foreach(GameObject testDot in allTestDots) {
            DestroyImmediate(testDot);
        }
    }
}
