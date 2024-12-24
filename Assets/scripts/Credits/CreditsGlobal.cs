using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CreditsGlobal : MonoBehaviour {
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private float scrollSpeed;
    private float usedScrollSpeed;
    private float stopDuration;
    [SerializeField] private float pauseTime;
    [SerializeField] private float stopTime;
    private bool autoScroll = false;
    private float lowestPosition;
    
    void Awake() {
        usedScrollSpeed = scrollSpeed;
        stopDuration = 0;
    }

    void Update() {
        TryScroll();
    }

    
    public void OnPointerDown() {
        StartCoroutine(StopScroll());
    }
    
    public void OnPointerUp() {
        StartCoroutine(StartScrolling());
    }

    public void OnPressEnterCreditsButton() {
        Utilities.TryGetComponent<Canvas>(creditsCanvas).sortingOrder = Constants.creditsCanvasSortOrder;
        MoveToInitialPosition();
        autoScroll = true;
    }

    public void OnPressExitCreditsButton() {
        Utilities.TryGetComponent<Canvas>(creditsCanvas).sortingOrder = Constants.deactivatedCanvasSortOrder;
        autoScroll = false;
    }

    private IEnumerator StopScroll() {
        yield return new WaitForSeconds(0.01f);
        stopDuration += 0.01f;
        usedScrollSpeed = scrollSpeed * ((stopTime-stopDuration)/stopTime);
        if(stopDuration >= stopTime) {
            usedScrollSpeed = 0;
            stopDuration = stopTime;
        }
        else {
            StartCoroutine(StopScroll());
        }
    }

    private IEnumerator StartScrolling() {
        yield return new WaitForSeconds(pauseTime);
        autoScroll = true;
        usedScrollSpeed = scrollSpeed;
        stopDuration = 0;
    }


    private void TryScroll() {
        if(!autoScroll) { return; }
        CalculateLowestPosition();
        if(lowestPosition > Screen.height*3/2) { MoveToInitialPosition(); }
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y+usedScrollSpeed, transform.position.z), 0.5f);
    }


    private void CalculateLowestPosition() {
        Transform lastCredit = transform.GetChild(transform.childCount-1);
        lowestPosition = lastCredit.position.y - Utilities.TryGetComponent<TextMeshProUGUI>(lastCredit.gameObject).fontSize;
        Debug.Log(lowestPosition);
    }

    private void MoveToInitialPosition() {
        transform.position = new Vector3(Screen.width/2, -Screen.height/2, 0);
    }
}
