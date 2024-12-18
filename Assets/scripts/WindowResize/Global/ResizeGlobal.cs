using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeGlobal : WindowSizeChange {
    [SerializeField] public bool renew = false;
    
    void Awake() {
        if(PrefabStageUtility.GetCurrentPrefabStage() != null) { return; }
        if(!Application.isPlaying) { return; }
        RenewAll();
    }
    
    void Update() {
        if(base.GetScreenSizeChange()) { 
            Debug.Log($"new window size: {Screen.width}, {Screen.height}");
            RenewAll();
        }
        
        //|---------------------------------------------------------------------------------|
        //|---------------------------------------------------------------------------------|
        //| WARNING: DO NOT REMOVE renew, OTHERWISE THE EDITOR FREEZES AND CRASHES RANDOMLY |
        //|---------------------------------------------------------------------------------|
        //|---------------------------------------------------------------------------------|

        else if(renew) {
            //InitializeAll();
            RenewAll();
            renew = false;
            Debug.Log("AAAAAA Renewed");
        }
    }

    public void RenewAll() {
        RelativeBase[] allRelativeBases = StageUtility.GetCurrentStageHandle().FindComponentsOfType<RelativeBase>();
        Debug.Log("DD allRelativeBases: "+allRelativeBases.Length);
        int maxRenewIndex = 0;
        foreach(RelativeBase relativeBase in allRelativeBases) {
            if(relativeBase.renewIndex > maxRenewIndex) {
                maxRenewIndex = relativeBase.renewIndex;
            }
        }
        for(int i=0; i<=maxRenewIndex; i++) {
            SizeRelative[] allSizeRelatives = StageUtility.GetCurrentStageHandle().FindComponentsOfType<SizeRelative>();
            foreach(SizeRelative sizeRelative in allSizeRelatives) {
                if(!sizeRelative.enabled) { continue; }
                if(sizeRelative.renewIndex == i) { sizeRelative.Renew(); }
            }
            ScaleRelative[] allScaleRelatives = StageUtility.GetCurrentStageHandle().FindComponentsOfType<ScaleRelative>();
            foreach(ScaleRelative scaleRelative in allScaleRelatives) {
                if(!scaleRelative.enabled) { continue; }
                if(scaleRelative.renewIndex == i) { scaleRelative.Renew(); }
            }
            MoveRelative[] allMoveRelatives = StageUtility.GetCurrentStageHandle().FindComponentsOfType<MoveRelative>();
            foreach(MoveRelative moveRelative in allMoveRelatives) {
                if(!moveRelative.enabled) { continue; }
                if(moveRelative.renewIndex == i) { moveRelative.Renew(); }
            }
        }

        //Renew ButtonOutlines
        StageUtility.GetCurrentStageHandle().FindComponentsOfType<ButtonsOutlineGlobal>().ToList().ForEach(obj => obj.Renew());
    }

    public void InitializeAll() {
        RelativeBase[] relativeBases = StageUtility.GetCurrentStageHandle().FindComponentsOfType<RelativeBase>();
        Debug.Log("DD relativeBases: "+relativeBases.Length);
        SizeRelative[] allSizeRelatives = StageUtility.GetCurrentStageHandle().FindComponentsOfType<SizeRelative>();
        Debug.Log("DD allSizeRelatives: "+allSizeRelatives.Length);
        foreach(SizeRelative sizeRelative in allSizeRelatives) {
            Debug.Log("CC found "+sizeRelative.name);
            sizeRelative.Renew();
        }
        ScaleRelative[] allScaleRelatives = StageUtility.GetCurrentStageHandle().FindComponentsOfType<ScaleRelative>();
        Debug.Log("DD allScaleRelatives: "+allScaleRelatives.Length);
        foreach(ScaleRelative scaleRelative in allScaleRelatives) {
            Debug.Log("CC found "+scaleRelative.name);
            scaleRelative.Renew();
        }
        MoveRelative[] allMoveRelatives = StageUtility.GetCurrentStageHandle().FindComponentsOfType<MoveRelative>();
        Debug.Log("DD allMoveRelatives: "+allMoveRelatives.Length);
        foreach(MoveRelative moveRelative in allMoveRelatives) {
            Debug.Log("CC found "+moveRelative.name);
            moveRelative.Renew();
        }
    }


}
