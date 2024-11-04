using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SocketAttributes : MonoBehaviour {
    [HideInInspector] public ElectricalStripData electricalStripData;
    [HideInInspector] public Transform[] childrenTransforms;
    [HideInInspector] public bool isActive = true;
    public Index2D id;

    void Awake() {
        childrenTransforms = Utilities.TryGetComponentsInChildren<Transform>(gameObject);
        electricalStripData = Utilities.TryGetComponentInParent<ElectricalStripData>(gameObject);
    }
}
