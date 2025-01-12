using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            foreach(ScriptTypeAndPlace scriptTypeAndPlace in priority.ScriptTypeAndPlace()) {
                if(scriptTypeAndPlace.GetScriptPlace() == InitPlace.All ||
                   (SceneManager.GetActiveScene().buildIndex == Constants.startBuildIndex         && scriptTypeAndPlace.GetScriptPlace() == InitPlace.Start) ||
                   (SceneManager.GetActiveScene().buildIndex == Constants.menuBuildIndex          && scriptTypeAndPlace.GetScriptPlace() == InitPlace.Menu) ||
                   (SceneManager.GetActiveScene().buildIndex == Constants.levelSelectorBuildIndex && scriptTypeAndPlace.GetScriptPlace() == InitPlace.LevelSelector) ||
                   (SceneManager.GetActiveScene().buildIndex >= Constants.firstLevelBuidIndex     && scriptTypeAndPlace.GetScriptPlace() == InitPlace.Level)) {
                

                    UnityEngine.Object[] objs = FindObjectsOfType(scriptTypeAndPlace.GetScriptType());
                    
                    Debug.Log("Found "+objs.Length+" instance(s) of ["+scriptTypeAndPlace.GetScriptType().Name+"]");

                    foreach(UnityEngine.Object obj in objs) {
                        ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)obj;
                        StartCoroutine(scriptInitializerBase.TrackCoroutine(scriptInitializerBase.Initialize()));
                    }
                    foreach(UnityEngine.Object obj in objs) {
                        ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)obj;
                        yield return new WaitUntil(() => scriptInitializerBase.Initialized);
                    }
                }
            }
            Debug.Log(">>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<");
        }
        yield return null;
    }
}
