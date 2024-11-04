using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class JointsData : MonoBehaviour {

    [HideInInspector] public DebugC debugC;
    [HideInInspector] public ControlsData controlsData;
    [HideInInspector] public JointsOpacityGlobal jointsOpacityGlobal;
    [HideInInspector] public ElectricalStripData electricalStripData;
    [SerializeField]  public GameObject jointPrefab;
    [HideInInspector] public bool cachedJointsEnabled = false;
    [SerializeField]  public bool jointsEnabled = false;
    [HideInInspector] public Vector2 cachedScreenSize;
    [HideInInspector] public Transform[,] jointsGrid;
    
    [SerializeField]  public Material jointMaterial;
    [HideInInspector] public float r = Constants.electricalStripBaseSize.x;
    [HideInInspector] public float s = Constants.electricalStripSeparatorSize;

    // Start is called before the first frame update
    void Awake() {
        debugC = DebugC.Get();
        controlsData = FindObjectOfType<ControlsData>();
        jointsOpacityGlobal = GetComponent<JointsOpacityGlobal>();
        electricalStripData = FindObjectOfType<ElectricalStripData>();
        cachedScreenSize = new Vector2(Screen.width, Screen.height);
        jointMaterial.color = Constants.jointColor;
    }
}
