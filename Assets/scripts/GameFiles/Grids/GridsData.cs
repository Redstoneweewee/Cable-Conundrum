using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsData : Singleton<GridsData> {
    [SerializeField]  public List<SocketsRow> socketsActiveGrid = new List<SocketsRow>();
    [HideInInspector] public Transform[,]     jointsGrid;          //From JointsController
    [HideInInspector] public Transform[,]     socketsGrid;         //From ELectricalStripController
    [HideInInspector] public int[,]           plugsGrid;           //From ELectricalStripController
    [HideInInspector] public int[,]           allCablesGrid;       //From ELectricalStripController
    [HideInInspector] public bool[,]          allObstaclesGrid;    //From IntersectionDetector

    [SerializeField] public GameObject jointsParent;
    [SerializeField] public GameObject socketsParent;
    [SerializeField] public GameObject jointPrefab;
    [SerializeField] public GameObject socketPrefab;


    //should be moved back, but referenced here
    //private CablesGridAttributes[,] cablesGrid;           //From CableGeneration
    //private bool[,]                obstaclesGrid;        //From Obstacle
    //public CablesGridAttributes[,]  CablesGrid        {get{return cablesGrid;       } set{cablesGrid        = value;}}
    //public bool[,]                 ObstaclesGrid     {get{return obstaclesGrid;    } set{obstaclesGrid     = value;}}

    public override IEnumerator Initialize() {
        yield return null;
    }
}