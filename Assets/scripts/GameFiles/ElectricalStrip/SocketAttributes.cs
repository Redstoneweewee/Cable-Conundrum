using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SocketAttributes : MonoBehaviour {
    [HideInInspector] public SocketHandler socketHandler;
    [HideInInspector] public Transform[] childrenTransforms;
    [HideInInspector] public bool isActive = true;
    public Index2D id;

    void Awake() {
        socketHandler      = Utilities.TryGetComponent<SocketHandler>(gameObject);
        childrenTransforms = Utilities.TryGetComponentsInChildren<Transform>(gameObject);
    }
}
