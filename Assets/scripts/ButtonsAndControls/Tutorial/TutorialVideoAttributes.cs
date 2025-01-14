using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoAttributes : ScriptInitializerBase {
    [Tooltip("The page index of the video in the tutorial tab.")]
    [SerializeField] [Range(0, 1)] public int isNormalVideoPlayer;
    [SerializeField] [Min(0)] public int tutorialPageIndex;
    [TextArea(5, 1)]
    [SerializeField] public string description;
    [HideInInspector] public RenderTexture renderTexture;
    [HideInInspector] public VideoPlayer videoPlayer;

    //Initialized in TutorialController
    public override IEnumerator Initialize() {
        if(TutorialData.Instance.useNormalVideoPlayers != isNormalVideoPlayer) {
            yield break;
        }
        videoPlayer = Utilities.TryGetComponent<VideoPlayer>(gameObject);
        renderTexture = videoPlayer.targetTexture;
        videoPlayer.Pause();
        videoPlayer.frame = 0;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
    }
}
