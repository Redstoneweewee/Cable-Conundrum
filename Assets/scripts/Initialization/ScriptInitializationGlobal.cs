using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptInitializationGlobal : Singleton<ScriptInitializationGlobal> {
    public bool ShouldUpdate { get; private set; } = false;
    public bool FinshedInitialization { get; private set; } = false;

    public override IEnumerator Initialize() {
        yield return null;
    }

    public IEnumerator InitializeAll() {
        FinshedInitialization = false;
        float startTime = Time.time;
        string debugText = "Initialization Log for Scene ["+SceneManager.GetActiveScene().name+"]:\n";
        Debug.Log("Initialization Log for Scene ["+SceneManager.GetActiveScene().name+"]:\n");
        
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
                    
                    float deltaTime = Time.time - startTime;
                    debugText += "\n["+deltaTime+" sec] Found "+objs.Length+" instance(s) of ["+scriptTypeAndPlace.GetScriptType().Name+"]\n";
                    Debug.Log("["+deltaTime+" sec] Found "+objs.Length+" instance(s) of ["+scriptTypeAndPlace.GetScriptType().Name+"]");

                    foreach(UnityEngine.Object obj in objs) {
                        ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)obj;
                        StartCoroutine(scriptInitializerBase.TrackCoroutine(scriptInitializerBase.Initialize()));
                    }
                    int num = 1;
                    foreach(UnityEngine.Object obj in objs) {
                        ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)obj;
                        yield return new WaitUntil(() => scriptInitializerBase.Initialized);
                        deltaTime = Time.time - startTime;
                        debugText += "["+deltaTime+" sec] Finished Initializing #"+num+" ["+scriptTypeAndPlace.GetScriptType().Name+"]\n";
                        Debug.Log("["+deltaTime+" sec] Finished Initializing #"+num+" ["+scriptTypeAndPlace.GetScriptType().Name+"]");
                        num++;
                    }
                }
            }
            debugText += "\n-------------------------------------------\n>>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<\n-------------------------------------------\n";
            Debug.Log(">>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<");
        }
        FinshedInitialization = true;
        ShouldUpdate = true;
        Debug.Log(debugText);
        yield return null;
    }

    public void SetUpdate(bool value) {
        //this is set to false in ScenesController
        ShouldUpdate = value;
    }
}
