using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Plug : MonoBehaviour {
    public static int idIncrement = 1;
    private int id;
    public DebugC DebugC {set; get;}
    private CableGeneration cableGeneration;


    public bool isPluggedIn = false;
    [SerializeField] private bool isObstacle = false;
    [SerializeField] private Obstacle obstacle;
    [SerializeField] private Vector2 plugSize = new Vector2(1, 1);
    [Tooltip("This is all the space on the joints grid that this plug takes up."+
    "These values are used in IntersectionDetector. "+
    "The first position should be the left-most position.")]
    [SerializeField] private List<Vector2> localJointPositionsTakenUp = new List<Vector2>();
    [Tooltip("This is the local offset of where the plug will snap onto socket(s). "+
    "These values are used to determine whether the plug can snap onto a socket & whether there is enough space on the electrical strip. "+
    "These values are used in PlugInteractions.")]
    [SerializeField] private List<Vector2> localSnapPositions = new List<Vector2>();
    [Tooltip("This is the location that the level will place the plugs relative to when the level is first initialized.")]
    [SerializeField] private Vector2 center;

    public int             Id                         {get{return id;                        } set{id                         = value;}}
    public CableGeneration CableGeneration            {get{return cableGeneration;           } set{cableGeneration            = value;}}
    public bool            IsObstacle                 {get{return isObstacle;                } set{isObstacle                 = value;}}
    public Obstacle        Obstacle                   {get{return obstacle;                  } set{obstacle                   = value;}}
    public Vector2         PlugSize                   {get{return plugSize;                  } set{plugSize                   = value;}}
    public List<Vector2>   LocalSnapPositions         {get{return localSnapPositions;        } set{localSnapPositions         = value;}}
    public List<Vector2>   LocalJointPositionsTakenUp {get{return localJointPositionsTakenUp;} set{localJointPositionsTakenUp = value;}}
    public Vector2         Center                     {get{return center;                    } set{center                     = value;}}
    public bool            IsPluggedIn                {get{return isPluggedIn;               } set{isPluggedIn                = value;}}


    // Start is called before the first frame update
    void Start() {
        cableGeneration = GetComponentInChildren<CableGeneration>();
        if(isObstacle) { obstacle = GetComponent<Obstacle>(); }
        DebugC = DebugC.Get();
        DebugC.Log("initialized "+transform.name);
        Id = idIncrement;
        DebugC.Log($"{transform.name}'s id: {Id}");
        idIncrement++;
    }

    // Update is called once per frame
    void Update() {
    }
}
