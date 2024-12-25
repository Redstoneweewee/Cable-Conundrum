using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScenesData : Singleton<ScenesData> {
    public Animator         crossFadeTransition;
    public float            crossFadeAnimationStartDuration;
    public float            crossFadeAnimationEndDuration;
    public bool             sceneFinishedLoading = false;
    public bool             animationIsFinished = false;



    public override void OnAwake() { }
}

