using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SocketAttributes : ScriptInitializerBase {
    [HideInInspector] public SocketHandler socketHandler;
    [HideInInspector] public Transform[] childrenTransforms;
    [HideInInspector] public bool isActive = true;
    public Index2D id;

    public override IEnumerator Initialize() {
        socketHandler      = Utilities.TryGetComponent<SocketHandler>(gameObject);
        childrenTransforms = Utilities.TryGetComponentsInChildren<Transform>(gameObject);
        yield return null;
    }
}
