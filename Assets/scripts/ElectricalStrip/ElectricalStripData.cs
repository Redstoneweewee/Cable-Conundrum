using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class ElectricalStripData : MonoBehaviour {
    [HideInInspector] public DebugC debugC;
    [HideInInspector] public Mouse   mouse = Mouse.current;

    [HideInInspector] public ElectricalStripController electricalStripController;
    [HideInInspector] public ElectricalStripSizeController electricalStripSizeController;
    [HideInInspector] public JointsData jointsData;

    [HideInInspector] public Transform[,] socketsGrid;
    [HideInInspector] public int[,] plugsGrid;      //contains the plug ids, starting from 1. A value of 0 means there is no plug.
    [HideInInspector] public int[,] allCablesGrid;  //contains the number of cables at each index. A value of 0 means there are no cables.

    [HideInInspector] public float r = Constants.electricalStripBaseSize.x;
    [HideInInspector] public float s = Constants.electricalStripSeparatorSize;
    [SerializeField]  public bool resetBoard = false;
    [SerializeField]  public GameObject socketPrefab;
    [SerializeField] [Range(1, 10)] public int width = 1;
    [SerializeField] [Range(1, 10)] public int height = 2;
    [SerializeField] public List<SocketsRow> cachedSocketsActiveGrid = new List<SocketsRow>();
    [SerializeField] public List<SocketsRow> socketsActiveGrid = new List<SocketsRow>();
    [SerializeField] public GameObject backgroundVisual;
    [SerializeField] public GameObject socketsParent;
    [SerializeField] public GameObject powerSwitch;

    [HideInInspector] public bool temporarilyModifiable = false;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Vector2 cachedMousePosition;
    [HideInInspector] public RectTransform rectangularTransform;
    [HideInInspector] public Vector2 size;


    void Awake() {
        debugC = DebugC.Get();
        electricalStripController = GetComponent<ElectricalStripController>();
        electricalStripSizeController = GetComponent<ElectricalStripSizeController>();
        jointsData = FindObjectOfType<JointsData>();

        rectangularTransform = backgroundVisual.GetComponent<RectTransform>();
        size = rectangularTransform.sizeDelta;
    }

}
