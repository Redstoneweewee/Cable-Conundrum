using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//A script that initialized skeleton grids that determine the positions of each element.
//Contains Joint-type and Socket-type grid skeletons.
public class GridsSkeleton : MonoBehaviour {
    [HideInInspector] public GridsModifier gridsModifier;
    [HideInInspector] public Vector2[,] jointsSkeletonGrid;
    [HideInInspector] public Vector2[,] socketsSkeletonGrid;

    void Awake() {
        gridsModifier = Utilities.TryGetComponent<GridsModifier>(gameObject);
    }

    public void Initialize() {
        gridsModifier = Utilities.TryGetComponent<GridsModifier>(gameObject);
        gridsModifier.DeleteAllTestDots();
        InitializeJointsSkeletonGrid();
        InitializeSocketsSkeletonGrid(gridsModifier.height, gridsModifier.width);
    }

    private void InitializeJointsSkeletonGrid() {
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        float step = LevelResizeGlobal.instance.jointDistance;
        Vector2 topLeft = center;

        //Moves topLeft to outside the screen based on jointDistance
        while(topLeft.x > step)                 { topLeft.x -= step; }
        while(topLeft.y < Screen.height - step) { topLeft.y += step; }
        topLeft.x -= step; 
        topLeft.y += step;

        int width  = (int)((Screen.width - topLeft.x)/step) + 2;
        int height = (int)(topLeft.y/step) + 2;
        Debug.Log($"width: {width}, height: {height}");
        jointsSkeletonGrid = new Vector2[height, width];
        //Fill jointsSkeletonGrid with positions & instantiate test dots at each position for testing
        for(int i=0; i<height; i++) {
            for(int j=0; j<width; j++) {
                jointsSkeletonGrid[i,j] = new Vector2(topLeft.x + (j*step), topLeft.y - (i*step));
                if(gridsModifier.testDotsActive) {
                    GameObject newDot = Instantiate(gridsModifier.testingDot, gridsModifier.testingDotCanvas.transform);
                    newDot.GetComponentInChildren<Image>().color = Color.red;
                    newDot.transform.position = new Vector2(jointsSkeletonGrid[i,j].x, jointsSkeletonGrid[i,j].y);
                }
            }
        }
    }

    private void InitializeSocketsSkeletonGrid(int height, int width) {
        float r = LevelResizeGlobal.instance.electricalStripBaseSize.x;
        float s = LevelResizeGlobal.instance.electricalStripSeparatorDistance;

        Vector2 newSize = new Vector2((LevelResizeGlobal.instance.electricalStripBaseSize.x + LevelResizeGlobal.instance.electricalStripSeparatorDistance)*width  + LevelResizeGlobal.instance.electricalStripSeparatorDistance, 
                                      (LevelResizeGlobal.instance.electricalStripBaseSize.y + LevelResizeGlobal.instance.electricalStripSeparatorDistance)*height + LevelResizeGlobal.instance.electricalStripSeparatorDistance);
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        socketsSkeletonGrid = new Vector2[height, width];
        Vector2 topLeft = new Vector2(center.x - newSize.x/2, center.y + newSize.y/2);
        
        for(int i=1; i<=height; i++) {
            for(int j=1; j<=width; j++) {
                socketsSkeletonGrid[i-1,j-1] = new Vector2(topLeft.x + r*(j - 0.5f) + s*j, topLeft.y - r*(i - 0.5f) - s*i);
                if(gridsModifier.testDotsActive) {
                    GameObject newDot = Instantiate(gridsModifier.testingDot, gridsModifier.testingDotCanvas.transform);
                    newDot.GetComponentInChildren<Image>().color = Color.blue;
                    newDot.transform.position = new Vector2(socketsSkeletonGrid[i-1,j-1].x, socketsSkeletonGrid[i-1,j-1].y);
                }
            }
        }
    }
}
