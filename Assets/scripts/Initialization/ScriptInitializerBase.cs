using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptInitializerBase : MonoBehaviour {
    public bool Initialized { get; private set; } = false;


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
}