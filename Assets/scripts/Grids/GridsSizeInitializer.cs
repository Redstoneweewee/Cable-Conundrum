using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//A script that initialized skeleton grids that determine the positions of each element.
//Contains Joint-type and Socket-type grid skeletons.
[ExecuteInEditMode]
public class GridsSizeInitializer : MonoBehaviour {
    [HideInInspector] public ElectricalStripData electricalStripData;
    [HideInInspector] public Vector2[,] jointsSkeletonGrid;
    [HideInInspector] public Vector2[,] socketsSkeletonGrid;
    [SerializeField] private bool renewGrids;
    [SerializeField] private bool testDotsActive;
    private bool cachedtestDotsActive;
    [SerializeField] private GameObject TestingDot;
    [SerializeField] private GameObject GameCanvas;

    void Awake() {
        electricalStripData = FindObjectOfType<ElectricalStripData>();
        Initialize();
    }
    void Update() {
        if(renewGrids || testDotsActive != cachedtestDotsActive) {
            Initialize();
            renewGrids = false;
            cachedtestDotsActive = testDotsActive;
        }
    }


    public void Initialize() {
        DeleteAllTestDots();
        InitializeJointsSkeletonGrid();
        InitializeSocketsSkeletonGrid(electricalStripData.height, electricalStripData.width);
    }

    private void DeleteAllTestDots() {
        GameObject[] allTestDots = GameObject.FindGameObjectsWithTag("TestDot");
        foreach(GameObject testDot in allTestDots) {
            DestroyImmediate(testDot);
        }
    }

    private void InitializeJointsSkeletonGrid() {
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        float step = Constants.jointDistance;
        Vector2 topLeft = center;

        //Moves topLeft to outside the screen based on jointDistance
        while(topLeft.x > step)                 { topLeft.x -= step; }
        while(topLeft.y < Screen.height - step) { topLeft.y += step; }
        topLeft.x -= step; 
        topLeft.y += step;

        int width  = (int)((Screen.width - topLeft.x)/step) + 2;
        int height = (int)(topLeft.y/step) + 2;
        jointsSkeletonGrid = new Vector2[height, width];

        //Fill jointsSkeletonGrid with positions & instantiate test dots at each position for testing
        for(int i=0; i<height; i++) {
            for(int j=0; j<width; j++) {
                jointsSkeletonGrid[i,j] = new Vector2(topLeft.x + (j*step), topLeft.y - (i*step));
                if(testDotsActive) {
                    GameObject newDot = Instantiate(TestingDot, GameCanvas.transform);
                    newDot.GetComponentInChildren<Image>().color = Color.red;
                    newDot.transform.position = new Vector2(jointsSkeletonGrid[i,j].x, jointsSkeletonGrid[i,j].y);
                }
            }
        }
    }

    private void InitializeSocketsSkeletonGrid(int height, int width) {
        float r = Constants.electricalStripBaseSize.x;
        float s = Constants.electricalStripSeparatorSize;

        Vector2 newSize = new Vector2((Constants.electricalStripBaseSize.x + Constants.electricalStripSeparatorSize)*width  + Constants.electricalStripSeparatorSize, 
                                      (Constants.electricalStripBaseSize.y + Constants.electricalStripSeparatorSize)*height + Constants.electricalStripSeparatorSize);
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        socketsSkeletonGrid = new Vector2[height, width];
        Vector2 topLeft = new Vector2(center.x - newSize.x/2, center.y + newSize.y/2);
        
        for(int i=1; i<=height; i++) {
            for(int j=1; j<=width; j++) {
                socketsSkeletonGrid[i-1,j-1] = new Vector2(topLeft.x + r*(j - 0.5f) + s*j, topLeft.y - r*(i - 0.5f) - s*i);
                if(testDotsActive) {
                    GameObject newDot = Instantiate(TestingDot, GameCanvas.transform);
                    newDot.GetComponentInChildren<Image>().color = Color.blue;
                    newDot.transform.position = new Vector2(socketsSkeletonGrid[i-1,j-1].x, socketsSkeletonGrid[i-1,j-1].y);
                }
            }
        }
    }
}
