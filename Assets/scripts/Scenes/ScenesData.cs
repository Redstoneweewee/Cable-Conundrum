using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScenesData : MonoBehaviour {
    public ScenesController scenesController;
    public Animator         crossFadeTransition;
    public float            crossFadeAnimationStartDuration;
    public float            crossFadeAnimationEndDuration;
    public bool             sceneFinishedLoading = false;
    public bool             animationIsFinished = false;



    void Awake() {
        scenesController = Utilities.TryGetComponent<ScenesController>(gameObject);
    }
}

