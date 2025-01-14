using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlugAttributes : ScriptInitializerBase {
    public static int idIncrement = 1;
    [HideInInspector] public int id;

    [HideInInspector] public PlugHandler               plugHandler;
    [HideInInspector] public CableParentAttributes     cableParentAttributes;
    [HideInInspector] public CableHandler              cableHandler;

    [SerializeField] public bool isPluggedIn = false;
    [SerializeField] public bool isObstacle = false;
    [SerializeField] public ObstacleAttributes obstacleAttributes;
    [Tooltip("PlugPriority is determined by how \"new\" the plug is in terms of game chapters. Higher number = higher priority")]
    [SerializeField] [Min(0)] public int plugPriority;
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
    public override IEnumerator Initialize() {
        plugHandler               = Utilities.TryGetComponent<PlugHandler>(gameObject);
        cableParentAttributes     = Utilities.TryGetComponentInChildren<CableParentAttributes>(gameObject);
        cableHandler              = Utilities.TryGetComponentInChildren<CableHandler>(gameObject);

        if(isObstacle) { obstacleAttributes = Utilities.TryGetComponent<ObstacleAttributes>(gameObject); }
        id = idIncrement;
        idIncrement++;

        DebugC.Instance.Log("initialized "+transform.name);
        DebugC.Instance.Log($"{transform.name}'s id: {id}");
        yield return null;
    }

}
