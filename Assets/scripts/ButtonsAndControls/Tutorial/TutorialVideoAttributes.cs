using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoAttributes : MonoBehaviour {
    [Tooltip("The page index of the video in the tutorial tab.")]
    [SerializeField] [Min(0)] public int tutorialPageIndex;
    [TextArea(5, 1)]
    [SerializeField] public string description;
    [HideInInspector] public RenderTexture renderTexture;
    [HideInInspector] public VideoPlayer videoPlayer;
    [HideInInspector] public bool        initialLoad = false;

    //Initialized in TutorialController
    public IEnumerator Initialize() {
        bool caught = false;
        try {
            videoPlayer = Utilities.TryGetComponent<VideoPlayer>(gameObject);
            renderTexture = videoPlayer.targetTexture;
            Debug.Log("1");
            videoPlayer.Pause();
            Debug.Log("2");
            videoPlayer.frame = 0;
            Debug.Log("3");
            videoPlayer.Prepare();
            Debug.Log("4");
        }
        catch(Exception) {
            videoPlayer = Utilities.TryGetComponent<VideoPlayer>(gameObject);
            renderTexture = videoPlayer.targetTexture;
            caught = true;
            Debug.Log("5");
        }
        yield return new WaitUntil(() => caught ? true : videoPlayer.isPrepared);
        initialLoad = true;
            Debug.Log("6");
    }
}
