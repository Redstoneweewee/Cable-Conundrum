using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class CableParentAttributes : MonoBehaviour {
    [HideInInspector] public CableHandler cableHandler;

    [HideInInspector] public Mouse mouse = Mouse.current;
    [HideInInspector] public DebugC debugC;

    
    [HideInInspector] public CablePrefabs              cablePrefabs;
    [HideInInspector] public GridsData                 gridsData;
    [HideInInspector] public GridsSkeleton             gridsSkeleton;
    [HideInInspector] public PlugAttributes            plugAttributes;
    [HideInInspector] public IntersectionController    intersectionController;
    [HideInInspector] public ElectricalStripData       electricalStripData;
    [HideInInspector] public ElectricalStripController electricalStripController;
    [HideInInspector] public JointsData                jointsData;

    /* Cables:
    * [ [0 ]UpLeft,    [1 ]UpRight,    [2 ]DownLeft,    [3 ]DownRight,    [4 ]LeftUp,    [5 ]LeftDown,    [6 ]RightUp,    [7 ]RightDown,   ]
    * [ [8 ]InUpLeft,  [9 ]InUpRight,  [10]InDownLeft,  [11]InDownRight,  [12]InLeftUp,  [13]InLeftDown,  [14]InRightUp,  [15]InRightDown, ]
    * [ [16]OutUpLeft, [17]OutUpRight, [18]OutDownLeft, [19]OutDownRight, [20]OutLeftUp, [21]OutLeftDown, [22]OutRightUp, [23]OutRightDown ]
    * Link: https://docs.google.com/document/d/1-T7I-lNiF93s7gjlgOsbJBzwmPMfbWuQ_Jdx-Hd63DM/edit?usp=sharing
    */
    
    [SerializeField]  public List<GameObject> initialCables;// = new List<GameObject>();
    [HideInInspector] public Vector3         cachedPosition;
    [SerializeField] public List<Transform> cables = new List<Transform>();
    //doesn't get used/generated until the player plugs the plug into a socket
    //numbers always contain 0; keep going up to find the next cable until the size of the grid
    [HideInInspector] public CablesGridAttributes[,] cableGrid; 
    [HideInInspector] public int          lastRotationCableIndex;
    [HideInInspector] public Directions   cachedStartingDirection;
    [HideInInspector] public Directions   cachedEndingDirection;
    [HideInInspector] public Index2D      cachedMouseGridIndex;
    //[HideInInspector] public Transform[,] cachedJointsGrid;
    [HideInInspector] public bool         finishedInitialization = false;
    [HideInInspector] public bool         renewSiblingCableIndices = false;


    //Are edited when cables are regenerated 
    [SerializeField]  public Directions       startingDirection;
    [SerializeField]  public Directions       endingDirection;

    void Awake() {
        debugC = DebugC.Get();
        cableHandler              = Utilities.TryGetComponent<CableHandler>(gameObject);
        plugAttributes            = Utilities.TryGetComponentInParent<PlugAttributes>(gameObject);
        cablePrefabs              = FindObjectOfType<CablePrefabs>();
        gridsData                 = FindObjectOfType<GridsData>();
        gridsSkeleton             = FindObjectOfType<GridsSkeleton>();
        intersectionController    = FindObjectOfType<IntersectionController>();
        electricalStripData       = FindObjectOfType<ElectricalStripData>();
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        jointsData                = FindObjectOfType<JointsData>();
    }
}
