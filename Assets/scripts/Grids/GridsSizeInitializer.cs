using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A script that initialized skeleton grids that determine the positions of each element.
//Contains Joint-type and Socket-type grid skeletons.
public class GridsSizeInitializer : MonoBehaviour {
    public Vector2[,] jointsSkeletonGrid;
    public Vector2[,] socketsSkeletonGrid;
    [SerializeField] private GameObject TestingDot;
    [SerializeField] private GameObject GameCanvas;

    void Start() {
        InitializeJointsSkeletonGrid();
    }

    public void InitializeJointsSkeletonGrid() {
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
        Debug.Log($"w: {width}, h: {height}");

        //Fill jointsSkeletonGrid with positions & instantiate test dots at each position for testing
        for(int i=0; i<height; i++) {
            for(int j=0; j<width; j++) {
                jointsSkeletonGrid[i,j] = new Vector2(topLeft.x + (j*step), topLeft.y - (i*step));
                Instantiate(TestingDot, GameCanvas.transform);
                TestingDot.transform.position = new Vector2(jointsSkeletonGrid[i,j].x-Screen.width/2, jointsSkeletonGrid[i,j].y-Screen.height/2);
            }
        }
    }

/*
    public void InitializeSocketsStartPosition(int width, int height) {
        Vector2 newSize = new Vector2((Constants.electricalStripBaseSize.x + Constants.electricalStripSeparatorSize)*width  + Constants.electricalStripSeparatorSize, 
                                      (Constants.electricalStripBaseSize.y + Constants.electricalStripSeparatorSize)*height + 2*Constants.electricalStripSeparatorSize + Constants.powerSwitchBaseSize.y);
        Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
        socketsStartPosition = new Vector2(center.x - newSize.x/2, center.y + newSize.y/2);
        Instantiate(TestingDot);
        TestingDot.transform.position = jointsStartPosition;
    }
    */
}
