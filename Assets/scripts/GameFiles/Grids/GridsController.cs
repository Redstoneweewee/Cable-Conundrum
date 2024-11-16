using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsController : MonoBehaviour {
    private GridsData D;
    private GridsSkeleton S;

    void Awake() {
        D = Utilities.TryGetComponent<GridsData>(gameObject);
        S = Utilities.TryGetComponent<GridsSkeleton>(gameObject);
        S.Initialize();
        Initialize();
    }

    public void Initialize() {
        D = Utilities.TryGetComponent<GridsData>(gameObject);
        S = Utilities.TryGetComponent<GridsSkeleton>(gameObject);
        InitializeJointsGrid();
        InitializeSocketsActiveGrid();
        RenewSocketsGrid();
        RenewPlugsGrid();
        RenewAllCablesGrid();
        RenewAllObstaclesGrid();

        D.Awake();
        D.electricalStripData.Awake();
        D.electricalStripSizeController.Initialize();
        D.electricalStripSizeController.ModifyBackgroundVisual();
    }

    public void RenewGrids() {
        RenewPlugsGrid();
        RenewAllCablesGrid();
        RenewAllObstaclesGrid();
    }

    private void InitializeJointsGrid() {
        D.jointsGrid = new Transform[S.jointsSkeletonGrid.GetLength(0), S.jointsSkeletonGrid.GetLength(1)];
        int childCount = D.jointsParent.transform.childCount;
        int index = 0;
        for(int i=0; i<D.jointsGrid.GetLength(0); i++) {
            for(int j=0; j<D.jointsGrid.GetLength(1); j++) {
                if(index >= childCount) {
                    GameObject newJoint = Instantiate(D.jointPrefab, D.jointsParent.transform);
                    newJoint.transform.position = S.jointsSkeletonGrid[i, j];
                    newJoint.name = "Joint"+(index+1);
                    D.jointsGrid[i, j] = newJoint.transform;
                }
                else {
                    D.jointsGrid[i, j] = D.jointsParent.transform.GetChild(index);
                    D.jointsGrid[i, j].transform.position = S.jointsSkeletonGrid[i, j];
                }
                index++;
            }
        }
    }

    private void InitializeSocketsActiveGrid() {
        if(D.socketsActiveGrid == null) {
            D.socketsActiveGrid.Clear();
            for(int i=0; i<S.socketsSkeletonGrid.GetLength(0); i++) {
                D.socketsActiveGrid.Add(new SocketsRow(S.socketsSkeletonGrid.GetLength(1)));
            }
        }
        else {
            List<SocketsRow> temp = new List<SocketsRow>();
            for(int i=0; i<D.socketsActiveGrid.Count; i++) { temp.Add(new SocketsRow(D.socketsActiveGrid[i], S.socketsSkeletonGrid.GetLength(1))); }

            D.socketsActiveGrid.Clear();
            for(int i=0; i<S.socketsSkeletonGrid.GetLength(0); i++) {
                if(i < temp.Count) { D.socketsActiveGrid.Add(new SocketsRow(temp[i], S.socketsSkeletonGrid.GetLength(1))); }
                else { D.socketsActiveGrid.Add(new SocketsRow(S.socketsSkeletonGrid.GetLength(1))); }
            }
        }
    }

    public void RenewSocketsGrid() {
        D.socketsGrid = new Transform[S.socketsSkeletonGrid.GetLength(0), S.socketsSkeletonGrid.GetLength(1)];
        
        int childCount = D.socketsParent.transform.childCount;
        int index = 0;

        for(int i=0; i<childCount; i++) {
            D.socketsParent.transform.GetChild(i).gameObject.SetActive(true);
            for(int j=0; j<D.socketsParent.transform.GetChild(i).childCount; j++) {
                D.socketsParent.transform.GetChild(i).GetChild(j).gameObject.SetActive(true);
            }
        }
        for(int i=0; i<D.socketsGrid.GetLength(0); i++) {
            for(int j=0; j<D.socketsGrid.GetLength(1); j++) {
                if(index >= childCount) {
                    GameObject newSocket = Instantiate(D.socketPrefab, D.socketsParent.transform);
                    newSocket.transform.position = S.socketsSkeletonGrid[i, j];
                    newSocket.name = "Socket"+(index+1);
                    D.socketsGrid[i, j] = newSocket.transform;
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).isActive = true;
                    Utilities.TryGetComponent<SocketAttributes>(newSocket).id = new Index2D(i, j);
                }
                else {
                    D.socketsGrid[i, j] = D.socketsParent.transform.GetChild(index);
                    D.socketsGrid[i, j].transform.position = S.socketsSkeletonGrid[i, j];
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).isActive = true;
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).id = new Index2D(i, j);
                }
                if(D.socketsActiveGrid[i].row[j] == false) { 
                    Debug.Log($"Socket At ({i}, {j}) is inactive."); 
                    for(int a=0; a<D.socketsGrid[i, j].childCount; a++) {
                        D.socketsGrid[i, j].GetChild(a).gameObject.SetActive(false);
                    }
                    D.socketsGrid[i, j].gameObject.SetActive(true);
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).isActive = false;
                    Utilities.TryGetComponent<SocketAttributes>(D.socketsGrid[i, j].gameObject).id = new Index2D(i, j);
                }
                index++;
            }
        }
        for(int i=index; i<childCount; i++) {
            D.socketsParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void RenewPlugsGrid() {
        D.plugsGrid = new int[S.jointsSkeletonGrid.GetLength(0), S.jointsSkeletonGrid.GetLength(1)];
        //Get all plugs in the level
        PlugAttributes[] allPlugAttributes = FindObjectsOfType<PlugAttributes>();

        foreach(PlugAttributes plugAttribute in allPlugAttributes) {
            if(plugAttribute.isPluggedIn) {
                foreach(Vector2 localPlugPositionsTakenUp in plugAttribute.localJointPositionsTakenUp) {
                    Vector2 position = new Vector2(plugAttribute.transform.position.x, plugAttribute.transform.position.y) + localPlugPositionsTakenUp;
                    Index2D gridIndex = CalculateJointsGridIndex(position);
                    D.plugsGrid[gridIndex.x, gridIndex.y] = plugAttribute.id;
                }
            }
        }
    }

    public void RenewAllCablesGrid() {
        CableParentAttributes[] allCableAttributes = FindObjectsOfType<CableParentAttributes>();
        D.allCablesGrid = new int[S.jointsSkeletonGrid.GetLength(0), S.jointsSkeletonGrid.GetLength(1)];
        foreach(CableParentAttributes cableParentAttribute in allCableAttributes) {
            if(cableParentAttribute.cableGrid == default(Array)) { Debug.LogWarning($"CableGrid of {cableParentAttribute?.transform?.name} is null."); continue; }
            for(int i=0; i<cableParentAttribute.cableGrid.GetLength(0); i++) {
                for(int j=0; j<cableParentAttribute.cableGrid.GetLength(1); j++) {
                    if(cableParentAttribute.cableGrid[i,j].hasCable) { D.allCablesGrid[i,j] += 1; }
                }
            }
        }
    }

    public void RenewAllObstaclesGrid() {
        ObstacleAttributes[] obstacleAttributes = FindObjectsOfType<ObstacleAttributes>();
        D.allObstaclesGrid = new bool[S.jointsSkeletonGrid.GetLength(0), S.jointsSkeletonGrid.GetLength(1)];

        foreach(ObstacleAttributes obstacleAttribute in obstacleAttributes) {
            if(obstacleAttribute.obstacleType == ObstacleTypes.Plug) { continue; }
            if(obstacleAttribute.obstacleGrid == default(Array)) { Debug.LogWarning($"{obstacleAttribute?.name}'s obstaclesGrid not defined."); continue; }
            for(int i=0; i<D.allObstaclesGrid.GetLength(0); i++) {
                for(int j=0; j<D.allObstaclesGrid.GetLength(1); j++) {
                    if(obstacleAttribute.obstacleGrid[i,j] == true) {
                        D.allObstaclesGrid[i,j] = true;
                    }
                }
            }
        }
    }




    private Index2D CalculateJointsGridIndex(Vector2 position) {
        float   subJointLength  = Constants.jointDistance/2;
        Vector2 distanceFromTopLeftJoint = new Vector2(position.x - S.jointsSkeletonGrid[0,0].x, S.jointsSkeletonGrid[0,0].y - position.y);
        Index2D gridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        gridIndex          = new Index2D(Math.Clamp(gridIndex.y, 0, S.jointsSkeletonGrid.GetLength(0)-1), Math.Clamp(gridIndex.x, 0, S.jointsSkeletonGrid.GetLength(1)-1));
        return gridIndex;
    }
}
