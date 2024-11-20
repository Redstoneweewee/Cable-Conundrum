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
        videoPlayer = Utilities.TryGetComponent<VideoPlayer>(gameObject);
        renderTexture = videoPlayer.targetTexture;
        videoPlayer.Pause();
        videoPlayer.frame = 0;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        initialLoad = true;
    }
}
