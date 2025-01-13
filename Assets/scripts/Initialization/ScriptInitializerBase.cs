using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptInitializerBase : MonoBehaviour {
    [Tooltip("A higher number = later initialization. "+
    "Those with the same number will initialize together in an undefined order.")]
    [Min(0)] private int initializationPriority = 0;
    public bool Initialized {get; private set; } = false;


    public abstract IEnumerator Initialize();

    //|----------------------------------------------|
    //| the only awake should be in ScenesController |
    //|----------------------------------------------|
    public void Awake() {
        //|----------------------------------------------|
        //| the only awake should be in ScenesController |
        //|----------------------------------------------|
    }
    //|----------------------------------------------|
    //| the only awake should be in ScenesController |
    //|----------------------------------------------|

    
    public IEnumerator TrackCoroutine(IEnumerator coroutine) {
        Initialized = false;
        yield return coroutine;
        Initialized = true;
    }

    public int GetInitializationPriority() {
        return initializationPriority;
    }
}