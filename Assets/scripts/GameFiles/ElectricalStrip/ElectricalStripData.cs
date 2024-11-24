using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[ExecuteInEditMode]
public class ElectricalStripData : MonoBehaviour {
    [HideInInspector] public ElectricalStripController electricalStripController;
    [HideInInspector] public Mouse   mouse = Mouse.current;

    [HideInInspector] public ElectricalStripSizeController electricalStripSizeController;
    [HideInInspector] public JointsData jointsData;
    [HideInInspector] public GridsModifier gridsModifier;

    //[HideInInspector] public Transform[,] socketsGrid;
    //[HideInInspector] public int[,] plugsGrid;      //contains the plug ids, starting from 1. A value of 0 means there is no plug.
    //[HideInInspector] public int[,] allCablesGrid;  //contains the number of cables at each index. A value of 0 means there are no cables.

    [HideInInspector] public float r = Constants.electricalStripBaseSize.x;
    [HideInInspector] public float s = Constants.electricalStripSeparatorSize;
    [SerializeField]  public GameObject socketPrefab;
    //[SerializeField] public List<SocketsRow> cachedSocketsActiveGrid = new List<SocketsRow>();
    //[SerializeField] public List<SocketsRow> socketsActiveGrid = new List<SocketsRow>();
    [SerializeField] public GameObject backgroundVisual;
    [SerializeField] public GameObject socketsParent;
    [SerializeField] public GameObject powerSwitch;

    [HideInInspector] public bool temporarilyModifiable = false;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Vector2 cachedMousePosition;
    [HideInInspector] public RectTransform rectangularTransform;


    public void Awake() {
        electricalStripController     = Utilities.TryGetComponent<ElectricalStripController>(gameObject);
        electricalStripSizeController = Utilities.TryGetComponent<ElectricalStripSizeController>(gameObject);
        jointsData                    = FindObjectOfType<JointsData>();
        gridsModifier                 = FindObjectOfType<GridsModifier>();

        rectangularTransform = Utilities.TryGetComponent<RectTransform>(backgroundVisual);
    }

}
