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
        int startFrame = Time.frameCount;
        float startTime = Time.time;
        string debugText = "Initialization Log for Scene ["+SceneManager.GetActiveScene().name+"]:\n";
        //Debug.Log("Initialization Log for Scene ["+SceneManager.GetActiveScene().name+"]:\n");
        
        List<ScriptInitializerBase> temp = FindObjectsByType<ScriptInitializerBase>(FindObjectsSortMode.None).ToList();
        //Initialize Singletons first
        Singleton[] tests = FindObjectsByType<Singleton>(FindObjectsSortMode.None);
        foreach(Singleton obj in tests) {
            if(!obj.IsSingletonInitialized) { obj.SingletonInitialization(); }
        }
        //Initialize the rest of scripts
        foreach(InitializationPriority priority in ScriptInitializationPriority.list) {
            foreach(ScriptTypeAndPlace scriptTypeAndPlace in priority.ScriptTypeAndPlace()) {
                if(scriptTypeAndPlace.GetScriptPlace() == InitPlace.All ||
                   (SceneManager.GetActiveScene().buildIndex == Constants.startBuildIndex         && scriptTypeAndPlace.GetScriptPlace() == InitPlace.Start) ||
                   (SceneManager.GetActiveScene().buildIndex == Constants.menuBuildIndex          && scriptTypeAndPlace.GetScriptPlace() == InitPlace.Menu) ||
                   (SceneManager.GetActiveScene().buildIndex == Constants.levelSelectorBuildIndex && scriptTypeAndPlace.GetScriptPlace() == InitPlace.LevelSelector) ||
                   (SceneManager.GetActiveScene().buildIndex >= Constants.firstLevelBuidIndex     && scriptTypeAndPlace.GetScriptPlace() == InitPlace.Level)) {
                

                    List<UnityEngine.Object> objs = FindObjectsOfType(scriptTypeAndPlace.GetScriptType()).ToList();
                    List<bool> isEachInitialized = new List<bool>( new bool[objs.Count] );
                    
                    int deltaFrame = Time.frameCount - startFrame;
                    float deltaTime = Time.time - startTime;
                    debugText += "\n["+deltaFrame+"/"+deltaTime+" frm] Found "+objs.Count+" instance(s) of ["+scriptTypeAndPlace.GetScriptType().Name+"]\n";
                    //Debug.Log("["+deltaFrame+"/"+deltaTime+" frm] Found "+objs.Count+" instance(s) of ["+scriptTypeAndPlace.GetScriptType().Name+"]");
                
                    int num = 1;
                    for(int i=objs.Count-1; i>=0; i--) {
                        ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)objs[i];
                        //if(scriptInitializerBase.Initialized) {
                        //    debugText += "["+deltaFrame+"/"+deltaTime+" frm] Already Initialized: #"+num+" ["+scriptTypeAndPlace.GetScriptType().Name+"]\n";
                        //    Debug.Log("["+deltaFrame+"/"+deltaTime+" frm] Already Initialized: #"+num+" ["+scriptTypeAndPlace.GetScriptType().Name+"]");
                        //    //objs.RemoveAt(i);
                        //    num++;
                        //    continue;
                        //}
                        StartCoroutine(scriptInitializerBase.TrackCoroutine(scriptInitializerBase.Initialize()));
                        num++;
                    }
                    //num = 1;
                    //for(int i=objs.Count-1; i>=0; i--) {
                    //    ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)objs[i];
                    //    yield return new WaitUntil(() => scriptInitializerBase.Initialized);
                    //    deltaFrame = Time.frameCount - startFrame;
                    //    deltaTime = Time.time - startTime;
                    //    debugText += "["+deltaFrame+"/"+deltaTime+" frm] Finished Initializing #"+num+" ["+scriptTypeAndPlace.GetScriptType().Name+"]\n";
                    //    //Debug.Log("["+deltaFrame+"/"+deltaTime+" frm] Finished Initializing #"+num+" ["+scriptTypeAndPlace.GetScriptType().Name+"]");
                    //    num++;
                    //}
                    bool cont = true;
                    while(cont) {
                        if(!AllInitialized(objs, ref debugText, startFrame, startTime, isEachInitialized)) {
                            yield return null;
                        }
                        else {
                            cont = false;
                        }
                    }
                }
            }
            debugText += "\n-------------------------------------------\n>>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<\n-------------------------------------------\n";
            //Debug.Log(">>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<");
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

    private bool AllInitialized(List<UnityEngine.Object> objs, ref string debugText, int startFrame = 0, float startTime = 0, List<bool> isEachInitialized = null) {
        //Debugging part - not needed in the actual function
        if(isEachInitialized != null) {
            for(int i=0; i<objs.Count; i++) {
                ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)objs[i];
                if(!isEachInitialized[i] && scriptInitializerBase.Initialized) {
                    int deltaFrame  = Time.frameCount - startFrame;
                    float deltaTime = Time.time - startTime;
                    debugText += "["+deltaFrame+"/"+deltaTime+" frm] Finished Initializing #"+(i+1)+" ["+objs[i].name+"]\n";
                    //Debug.Log("["+(Time.frameCount-startFrame)+" frm] Finished Initializing #"+(i+1)+" ["+objs[i].name+"]\n");
                }
                isEachInitialized[i] = scriptInitializerBase.Initialized;
            }
            
        }
        //Optimized part
        for(int i=0; i<objs.Count; i++) {
            ScriptInitializerBase scriptInitializerBase = (ScriptInitializerBase)objs[i];
            if(!scriptInitializerBase.Initialized) {
                return false;
            }
        }
        return true;
    }
}
