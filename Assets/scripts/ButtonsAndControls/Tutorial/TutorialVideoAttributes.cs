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

    //Initialized in TutorialController
    public void Initialize() {
        videoPlayer = Utilities.TryGetComponent<VideoPlayer>(gameObject);
        renderTexture = videoPlayer.targetTexture;
        videoPlayer.Stop();
        videoPlayer.Prepare();
    }
}
