using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsData : MonoBehaviour {
                     private List<SocketsRow> cachedSocketsActiveGrid = new List<SocketsRow>(); //From ELectricalStripSizeController
    [SerializeField] private List<SocketsRow> socketsActiveGrid = new List<SocketsRow>();
    private Transform[,]           jointsGrid;          //From JointsController
    private Transform[,]           socketsGrid;         //From ELectricalStripController
    private int[,]                 plugsGrid;           //From ELectricalStripController
    private int[,]                 allCablesGrid;       //From ELectricalStripController
    private bool[,]                allObstaclesGrid;    //From IntersectionDetector


    public Transform[,]           JointsGrid       {get{return jointsGrid;      } set{jointsGrid       = value;}}
    public Transform[,]           SocketsGrid      {get{return socketsGrid;     } set{socketsGrid      = value;}}
    public int[,]                 PlugsGrid        {get{return plugsGrid;       } set{plugsGrid        = value;}}
    public int[,]                 AllCablesGrid    {get{return allCablesGrid;   } set{allCablesGrid    = value;}}
    public bool[,]                AllObstaclesGrid {get{return allObstaclesGrid;} set{allObstaclesGrid = value;}}




    //should be moved back, but referenced here
    private CablesGridAttributes[,] cablesGrid;           //From CableGeneration
    private bool[,]                obstaclesGrid;        //From Obstacle
    public CablesGridAttributes[,]  CablesGrid        {get{return cablesGrid;       } set{cablesGrid        = value;}}
    public bool[,]                 ObstaclesGrid     {get{return obstaclesGrid;    } set{obstaclesGrid     = value;}}
}