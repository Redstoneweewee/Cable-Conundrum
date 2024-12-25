using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class JointsData : Singleton<JointsData> {
    [SerializeField]  public GameObject jointPrefab;
    [HideInInspector] public bool cachedJointsEnabled = false;
    [SerializeField]  public bool jointsEnabled = false;
    [HideInInspector] public Vector2 cachedScreenSize;
    //[HideInInspector] public Transform[,] jointsGrid;
    
    [SerializeField]  public Material jointMaterial;
    [HideInInspector] public float r;
    [HideInInspector] public float s;

    public override void OnAwake() {
        r = LevelResizeGlobal.Instance.electricalStripBaseSize.x;
        s = LevelResizeGlobal.Instance.electricalStripSeparatorDistance;
        cachedScreenSize    = new Vector2(Screen.width, Screen.height);
        jointMaterial.color = Constants.jointColor;
    }
}
