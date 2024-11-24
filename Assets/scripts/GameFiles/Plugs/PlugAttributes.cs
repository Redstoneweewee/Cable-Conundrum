using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlugAttributes : MonoBehaviour {
    public static int idIncrement = 1;
    [HideInInspector] public Mouse mouse = Mouse.current;
    [HideInInspector] public int id;

    [HideInInspector] public GridsData                 gridsData;
    [HideInInspector] public GridsController           gridsController;
    [HideInInspector] public PlugHandler               plugHandler;
    [HideInInspector] public ControlsData              controlsData;
    [HideInInspector] public ElectricalStripData       electricalStripData;
    [HideInInspector] public ElectricalStripController electricalStripController;
    [HideInInspector] public IntersectionData          intersectionData;
    [HideInInspector] public IntersectionController    intersectionController;
    [HideInInspector] public JointsData                jointsData;
    [HideInInspector] public CableParentAttributes     cableParentAttributes;
    [HideInInspector] public CableHandler              cableHandler;
    [HideInInspector] public SoundsData                soundsData;

    [SerializeField] public bool isPluggedIn = false;
    [SerializeField] public bool isObstacle = false;
    [SerializeField] public ObstacleAttributes obstacleAttributes;
    [SerializeField] public Vector2 plugSize = new Vector2(1, 1);
    [Tooltip("This is all the space on the joints grid that this plug takes up. These values are used in IntersectionDetector. The first position should be the left-most position.")]
    [SerializeField] public List<Vector2> localJointPositionsTakenUp = new List<Vector2>();

    [Tooltip("This is the local offset of where the plug will snap onto socket(s). These values are used to determine whether the plug can snap onto a socket & whether there is enough space on the electrical strip. These values are used in PlugInteractions.")]
    [SerializeField] public List<Vector2> localSnapPositions = new List<Vector2>();

    [Tooltip("This is the location that the level will place the plugs relative to when the level is first initialized.")]
    [SerializeField] public Vector2 center;
    [SerializeField] public GameObject plugVisual;


    [HideInInspector] public IEnumerator dragCoroutine;
    [HideInInspector] public IEnumerator modifyCableCoroutine;
    [HideInInspector] public bool    isDragging         = false;
    [HideInInspector] public bool    isModifyingCables  = false;

    [HideInInspector] public Vector2 cachedPlugPositionDynamic = Vector2.zero;
    [HideInInspector] public Vector2 targetPosition     = Vector2.zero;
    [HideInInspector] public Vector2 offset             = Vector2.zero;


    // Start is called before the first frame update
    void Awake() {
        plugHandler               = Utilities.TryGetComponent<PlugHandler>(gameObject);
        gridsData                 = FindObjectOfType<GridsData>();
        gridsController           = FindObjectOfType<GridsController>();
        controlsData              = FindObjectOfType<ControlsData>();
        electricalStripData       = FindObjectOfType<ElectricalStripData>();
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        intersectionData          = FindObjectOfType<IntersectionData>();
        intersectionController    = FindObjectOfType<IntersectionController>();
        jointsData                = FindObjectOfType<JointsData>();
        soundsData                = FindObjectOfType<SoundsData>();
        cableParentAttributes     = Utilities.TryGetComponentInChildren<CableParentAttributes>(gameObject);
        cableHandler              = Utilities.TryGetComponentInChildren<CableHandler>(gameObject);

        if(isObstacle) { obstacleAttributes = Utilities.TryGetComponent<ObstacleAttributes>(gameObject); }
        id = idIncrement;
        idIncrement++;

        DebugC.Get()?.Log("initialized "+transform.name);
        DebugC.Get()?.Log($"{transform.name}'s id: {id}");
    }

}
