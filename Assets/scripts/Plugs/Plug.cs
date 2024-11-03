using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Plug : MonoBehaviour {
    public static int idIncrement = 1;
    public DebugC DebugC {set; get;}
    [HideInInspector] public int id;
    [HideInInspector] public CableAttributes cableAttributes;


    public bool isPluggedIn = false;
    [SerializeField] public bool isObstacle = false;
    [SerializeField] public Obstacle obstacle;
    [SerializeField] public Vector2 plugSize = new Vector2(1, 1);
    [Tooltip("This is all the space on the joints grid that this plug takes up."+
    "These values are used in IntersectionDetector. "+
    "The first position should be the left-most position.")]
    [SerializeField] public List<Vector2> localJointPositionsTakenUp = new List<Vector2>();
    [Tooltip("This is the local offset of where the plug will snap onto socket(s). "+
    "These values are used to determine whether the plug can snap onto a socket & whether there is enough space on the electrical strip. "+
    "These values are used in PlugInteractions.")]
    [SerializeField] public List<Vector2> localSnapPositions = new List<Vector2>();
    [Tooltip("This is the location that the level will place the plugs relative to when the level is first initialized.")]
    [SerializeField] public Vector2 center;


    // Start is called before the first frame update
    void Start() {
        cableAttributes = GetComponentInChildren<CableAttributes>();
        if(isObstacle) { obstacle = GetComponent<Obstacle>(); }
        DebugC = DebugC.Get();
        DebugC.Log("initialized "+transform.name);
        id = idIncrement;
        DebugC.Log($"{transform.name}'s id: {id}");
        idIncrement++;
    }

    // Update is called once per frame
    void Update() {
    }
}
