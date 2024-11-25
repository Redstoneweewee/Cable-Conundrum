using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeGlobal : MonoBehaviour {
    private Vector2 cachedScreenSize;
    [SerializeField] public bool renew = false;
    
    void Awake() {
        RenewAll();
    }
    
    void Update() {
        if(Application.isPlaying && cachedScreenSize != new Vector2(Screen.width, Screen.height)) {
            RenewAll();
        }
        
        //|---------------------------------------------------------------------------------|
        //|---------------------------------------------------------------------------------|
        //| WARNING: DO NOT REMOVE renew, OTHERWISE THE EDITOR FREEZES AND CRASHES RANDOMLY |
        //|---------------------------------------------------------------------------------|
        //|---------------------------------------------------------------------------------|

        else if(renew) {
            RenewAll();
            renew = false;
        }
    }

    public void RenewAll() {
        RelativeBase[] allRelativeBases = FindObjectsOfType<RelativeBase>();
        int maxRenewIndex = 0;
        foreach(RelativeBase relativeBase in allRelativeBases) {
            if(relativeBase.renewIndex > maxRenewIndex) {
                maxRenewIndex = relativeBase.renewIndex;
            }
        }
        for(int i=0; i<=maxRenewIndex; i++) {
            SizeRelative[] allSizeRelatives = FindObjectsOfType<SizeRelative>();
            foreach(SizeRelative sizeRelative in allSizeRelatives) {
                if(sizeRelative.renewIndex == i) { sizeRelative.Renew(); }
            }
            ScaleRelative[] allScaleRelatives = FindObjectsOfType<ScaleRelative>();
            foreach(ScaleRelative scaleRelative in allScaleRelatives) {
                if(scaleRelative.renewIndex == i) { scaleRelative.Renew(); }
            }
            MoveRelative[] allMoveRelatives = FindObjectsOfType<MoveRelative>();
            foreach(MoveRelative moveRelative in allMoveRelatives) {
                if(moveRelative.renewIndex == i) { moveRelative.Renew(); }
            }
        }
    }
}
