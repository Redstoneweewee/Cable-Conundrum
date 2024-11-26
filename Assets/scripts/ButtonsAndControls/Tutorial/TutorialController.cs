using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {
    TutorialData D;

    void Awake() {
        D = Utilities.TryGetComponent<TutorialData>(gameObject);
    }

    //is initialized in the initalizerBase
    public IEnumerator Initialize() {
        TutorialVideoAttributes[] temp = FindObjectsByType<TutorialVideoAttributes>(FindObjectsSortMode.None);
        D.videoPlayers = new TutorialVideoAttributes[temp.Length];
        foreach(TutorialVideoAttributes tutorialVideoAttributes in temp) {
            StartCoroutine(tutorialVideoAttributes.Initialize());
            yield return new WaitUntil(() => tutorialVideoAttributes.initialLoad);
            D.videoPlayers[tutorialVideoAttributes.tutorialPageIndex] = tutorialVideoAttributes;
        }
        StartCoroutine(SetVideoDisplayAndDescription(0, 0));
        yield return new WaitUntil(() => D.initialVideoInitialized);
        D.isInitialized = true;
    }


    public void OnPressEnterTutorialPageButton() {
        D.tutorialCanvas.SetActive(true);
        StartCoroutine(SetVideoDisplayAndDescription(D.currentPageIndex, D.currentPageIndex));
    }
    public void OnPressExitTutorialPageButton() {
        D.tutorialCanvas.SetActive(false);
    }

    public void OnPressNextTutorialPageButton() {
        if(D.currentPageIndex >= D.videoPlayers.Length-1) { return; }
        D.previousTutorialPageButton.SetActive(true);
        D.currentPageIndex++;
        StartCoroutine(SetVideoDisplayAndDescription(D.currentPageIndex, D.currentPageIndex-1));

        if(D.currentPageIndex == D.videoPlayers.Length-1) { D.nextTutorialPageButton.SetActive(false); }
    }
    public void OnPressPreviousTutorialPageButton() {
        if(D.currentPageIndex == 0) { return; }
        D.nextTutorialPageButton.SetActive(true);
        D.currentPageIndex--;
        StartCoroutine(SetVideoDisplayAndDescription(D.currentPageIndex, D.currentPageIndex+1));

        if(D.currentPageIndex == 0) { D.previousTutorialPageButton.SetActive(false); }
    }

    private IEnumerator SetVideoDisplayAndDescription(int index, int previousIndex) {
        Utilities.TryGetComponent<TextMeshProUGUI>(D.description).text = D.videoPlayers[index].description;
        D.videoPlayers[previousIndex].videoPlayer.Pause();
        D.videoPlayers[previousIndex].videoPlayer.frame = 0;
        D.videoPlayers[index].videoPlayer.Prepare();
        D.videoPlayers[index].videoPlayer.Play();
        yield return new WaitUntil(() => D.videoPlayers[index].videoPlayer.isPrepared);
        Utilities.TryGetComponent<RawImage>(D.videoDisplay).texture = D.videoPlayers[index].renderTexture;
        D.initialVideoInitialized = true;
    }
}
