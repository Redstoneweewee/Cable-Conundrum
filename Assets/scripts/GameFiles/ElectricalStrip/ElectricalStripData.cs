using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[ExecuteInEditMode]
public class ElectricalStripData : Singleton<ElectricalStripData> {
    [HideInInspector] public Mouse   mouse = Mouse.current;

    [HideInInspector] public float r;
    [HideInInspector] public float s;
    [SerializeField] public GameObject backgroundVisual;
    [SerializeField] public GameObject socketsParent;
    [SerializeField] public GameObject powerSwitch;

    [HideInInspector] public bool temporarilyModifiable = false;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Vector2 cachedMousePosition;
    [HideInInspector] public RectTransform rectangularTransform;

    public override void OnAwake() {
        r = LevelResizeGlobal.Instance.electricalStripBaseSize.x;
        s = LevelResizeGlobal.Instance.electricalStripSeparatorDistance;
        rectangularTransform = Utilities.TryGetComponent<RectTransform>(backgroundVisual);

    }
}
