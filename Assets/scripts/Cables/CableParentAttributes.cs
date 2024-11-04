using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CableParentAttributes : MonoBehaviour {

    [HideInInspector] public Mouse mouse = Mouse.current;
    [HideInInspector] public DebugC debugC;

    [HideInInspector] public Plug                      plug;
    [HideInInspector] public PlugInteractions          plugInteractions;
    [HideInInspector] public IntersectionController    intersectionController;
    [HideInInspector] public ElectricalStripData       electricalStripData;
    [HideInInspector] public ElectricalStripController electricalStripController;
    [HideInInspector] public JointsData          jointsData;

    /* Cables:
    * [ [0 ]UpLeft,    [1 ]UpRight,    [2 ]DownLeft,    [3 ]DownRight,    [4 ]LeftUp,    [5 ]LeftDown,    [6 ]RightUp,    [7 ]RightDown,   ]
    * [ [8 ]InUpLeft,  [9 ]InUpRight,  [10]InDownLeft,  [11]InDownRight,  [12]InLeftUp,  [13]InLeftDown,  [14]InRightUp,  [15]InRightDown, ]
    * [ [16]OutUpLeft, [17]OutUpRight, [18]OutDownLeft, [19]OutDownRight, [20]OutLeftUp, [21]OutLeftDown, [22]OutRightUp, [23]OutRightDown ]
    * Link: https://docs.google.com/document/d/1-T7I-lNiF93s7gjlgOsbJBzwmPMfbWuQ_Jdx-Hd63DM/edit?usp=sharing
    */
    [SerializeField]  public GameObject[]    cablePrefabs;
    [SerializeField]  public GameObject[]    initialCables;
    [HideInInspector] public Vector3         cachedPosition;
    [HideInInspector] public List<Transform> cables = new List<Transform>();
    //doesn't get used/generated until the player plugs the plug into a socket
    //numbers always contain 0; keep going up to find the next cable until the size of the grid
    [HideInInspector] public CablesGridAttributes[,] cableGrid; 
    [HideInInspector] public int          lastRotationCableIndex;
    [HideInInspector] public Directions   cachedStartingDirection;
    [HideInInspector] public Directions   cachedEndingDirection;
    [HideInInspector] public Index2D      cachedMouseGridIndex;
    [HideInInspector] public Transform[,] cachedJointsGrid;


    //Are edited when cables are regenerated 
    [HideInInspector] public bool             isInitialCable;
    [HideInInspector] public bool             isRotationCable;
    [HideInInspector] public bool             isIntersectionCable;
    [HideInInspector] public CableTypes       cableType;
    [HideInInspector] public Image            cableImage;
    [HideInInspector] public float            zRotation;
    [HideInInspector] public Vector2          pivot;
    [HideInInspector] public ShadowDirections shadowDirection;
    [SerializeField]  public Directions       startingDirection;
    [SerializeField]  public Directions       endingDirection;
    [HideInInspector] public Vector2          directionMultiple;

    void Awake() {
        debugC = DebugC.Get();
        plug                      = GetComponentInParent<Plug>();
        plugInteractions          = GetComponentInParent<PlugInteractions>();
        intersectionController    = FindObjectOfType<IntersectionController>();
        electricalStripData       = FindObjectOfType<ElectricalStripData>();
        electricalStripController = electricalStripData.electricalStripController;
        jointsData                = FindObjectOfType<JointsData>();
    }
}
