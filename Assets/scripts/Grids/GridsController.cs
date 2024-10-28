using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsController : MonoBehaviour {
    //From JointsController
    private Transform[,] jointsGrid;
    public Transform[,] JointsGrid { get{return jointsGrid;} private set{jointsGrid = value;} }

    //From ELectricalStripController
    public Transform[,] SocketsGrid { get; set; }
    public int[,] PlugsGrid { get; set; }  //contains the plug ids, starting from 1. A value of 0 means there is no plug.
    public int[,] AllCablesGrid { get; set; }  //contains the number of cables at each index. A value of 0 means there are no cables.


    //From ELectricalStripSizeController
    [SerializeField] List<SocketsRow> cachedSocketsActiveGrid = new List<SocketsRow>();
    [SerializeField] List<SocketsRow> socketsActiveGrid = new List<SocketsRow>();


    //From CableGeneration
    private CableGridAttributes[,] cableGrid; 
    public CableGridAttributes[,] CableGrid {get{return cableGrid;} set{cableGrid = value;}}


    //From IntersectionDetector
    private bool[,] allObstaclesGrid;
    public bool[,] AllObstaclesGrid {get{return allObstaclesGrid;} set{allObstaclesGrid = value;}}


    //From Obstacle
    private bool[,] obstacleGrid;
    public bool[,]      ObstacleGrid          { get { return obstacleGrid;          } set { obstacleGrid          = value; } }



    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
