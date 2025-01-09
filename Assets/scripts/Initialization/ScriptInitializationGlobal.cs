using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScriptInitializationGlobal : MonoBehaviour {
    //|--------------------------------------------------------------|
    //|                                                              |
    //| This is the only script that should have an Awake function!! |
    //|                                                              |
    //|--------------------------------------------------------------|
    
    List<ScriptInitializerBase> sortedScriptInitializers;

    void Awake() {
        StartCoroutine(InitializeAll());
    }

    private IEnumerator InitializeAll() {
        List<ScriptInitializerBase> temp = FindObjectsByType<ScriptInitializerBase>(FindObjectsSortMode.None).ToList();

        //Initialize Singletons first
        Singleton[] tests = FindObjectsByType<Singleton>(FindObjectsSortMode.None);
        foreach(Singleton obj in tests) {
            obj.SingletonInitialization();
        }
        //Initialize the rest of scripts
        foreach(InitializationPriority priority in ScriptInitializationPriority.list) {
            foreach(Type type in priority.GetTypes()) {
                Debug.Log("Checking type ["+type.Name+"]");
                UnityEngine.Object[] objs = FindObjectsOfType(type);
                foreach(UnityEngine.Object obj in objs) {
                    ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)obj;
                    StartCoroutine(scriptInitializerBase.TrackCoroutine(scriptInitializerBase.Initialize()));
                }
                foreach(UnityEngine.Object obj in objs) {
                    ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)obj;
                    yield return new WaitUntil(() => scriptInitializerBase.Initialized);
                }
            }
            Debug.Log(">>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<");
        }
        yield return null;
    }
}
