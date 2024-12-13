using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            TutorialVideoAttributes[] temp = FindObjectsByType<TutorialVideoAttributes>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            //Debug.Log(temp.Length);
            D.videoPlayers = new TutorialVideoAttributes[temp.Length];
            foreach(TutorialVideoAttributes tutorialVideoAttributes in temp) {
                //If videos don't load, gets stuck here
                StartCoroutine(tutorialVideoAttributes.Initialize());
                yield return new WaitUntil(() => tutorialVideoAttributes.initialLoad);
                D.videoPlayers[tutorialVideoAttributes.tutorialPageIndex] = tutorialVideoAttributes;
            }
            StartCoroutine(SetVideoDisplayAndDescription(0, 0));
            yield return new WaitUntil(() => D.initialVideoInitialized);
            D.isInitialized = true;
    }


    public void OnPressEnterTutorialPageButton() {
        FindFirstObjectByType<ResizeGlobal>().RenewAll();
        Utilities.TryGetComponent<Canvas>(D.tutorialCanvas).sortingOrder = Constants.tutorialCanvasSortOrder;
        StartCoroutine(SetVideoDisplayAndDescription(D.currentPageIndex, D.currentPageIndex));
    }
    public void OnPressExitTutorialPageButton() {
        FindFirstObjectByType<ResizeGlobal>().RenewAll();
        Utilities.TryGetComponent<Canvas>(D.tutorialCanvas).sortingOrder = Constants.deactivatedCanvasSortOrder;
    }

    public void OnPressNextTutorialPageButton() {
        if(D.currentPageIndex >= D.videoPlayers.Length-1) { return; }
        Utilities.TryGetComponentInChildren<Button>(D.previousTutorialPageButton).enabled = true;
        Utilities.TryGetComponentsInChildren<Image>(D.previousTutorialPageButton).ToList().ForEach(image => image.enabled = true);

        D.currentPageIndex++;
        StartCoroutine(SetVideoDisplayAndDescription(D.currentPageIndex, D.currentPageIndex-1));

        if(D.currentPageIndex == D.videoPlayers.Length-1) { 
        Utilities.TryGetComponentInChildren<Button>(D.nextTutorialPageButton).enabled = false;
        Utilities.TryGetComponentsInChildren<Image>(D.nextTutorialPageButton).ToList().ForEach(image => image.enabled = false);
        }
    }
    public void OnPressPreviousTutorialPageButton() {
        if(D.currentPageIndex == 0) { return; }
        Utilities.TryGetComponentInChildren<Button>(D.nextTutorialPageButton).enabled = true;
        Utilities.TryGetComponentsInChildren<Image>(D.nextTutorialPageButton).ToList().ForEach(image => image.enabled = true);
        D.currentPageIndex--;
        StartCoroutine(SetVideoDisplayAndDescription(D.currentPageIndex, D.currentPageIndex+1));

        if(D.currentPageIndex == 0) { 
        Utilities.TryGetComponentInChildren<Button>(D.previousTutorialPageButton).enabled = false;
        Utilities.TryGetComponentsInChildren<Image>(D.previousTutorialPageButton).ToList().ForEach(image => image.enabled = false);
        }
    }

    private IEnumerator SetVideoDisplayAndDescription(int index, int previousIndex) {
        try {
            Utilities.TryGetComponent<TextMeshProUGUI>(D.description).text = D.videoPlayers[index].description;
            D.videoPlayers[previousIndex].videoPlayer.Pause();
            D.videoPlayers[previousIndex].videoPlayer.frame = 0;
            D.videoPlayers[index].videoPlayer.Prepare();
            D.videoPlayers[index].videoPlayer.Play();
        }
        catch(Exception e) {
            Debug.LogWarning("tutorial was unable to load. Error: "+e);
            D.initialVideoInitialized = true;
        }
        yield return new WaitUntil(() => D.videoPlayers[index].videoPlayer.isPrepared);
        Utilities.TryGetComponent<RawImage>(D.videoDisplay).texture = D.videoPlayers[index].renderTexture;
        D.initialVideoInitialized = true;
    }
}
