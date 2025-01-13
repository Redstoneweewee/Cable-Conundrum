using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CreditsGlobal : Singleton<CreditsGlobal> {
    [HideInInspector] public InputActionReference   mouseScrollAction;

    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float pauseTime;
    private bool isActivated = false;
    private bool autoScroll = false;
    private float lowestPosition;
    private IEnumerator startScrollCoroutine;
    
    public override IEnumerator Initialize() {
        mouseScrollAction = ControlsData.Instance.mouseScrollAction;
        yield return null;
    }

    void Update() {
        if(!isActivated) { return; }
        Vector2 mouseScroll = ControlsController.Instance.GetActionInputValue<Vector2>(mouseScrollAction);
        if(mouseScroll.y > 0) { 
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y-scrollSpeed*25*Utilities.TryGetComponent<Canvas>(creditsCanvas).scaleFactor, transform.position.z), 0.5f);
            if(startScrollCoroutine != null) { StopCoroutine(startScrollCoroutine); startScrollCoroutine = null; }
            StopScroll();
        }
        else if(mouseScroll.y < 0) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y+scrollSpeed*25*Utilities.TryGetComponent<Canvas>(creditsCanvas).scaleFactor, transform.position.z), 0.5f);
            if(startScrollCoroutine != null) { StopCoroutine(startScrollCoroutine); startScrollCoroutine = null; }
            StopScroll();
        }
        else {
            if(startScrollCoroutine == null && autoScroll == false) {
                startScrollCoroutine = StartScrolling();
                StartCoroutine(startScrollCoroutine);
            }
        }
        TryScroll();
    }

    public void OnPressEnterCreditsButton() {
        Utilities.TryGetComponent<Canvas>(creditsCanvas).sortingOrder = Constants.creditsCanvasSortOrder;
        MoveToInitialPosition();
        isActivated = true;
        autoScroll = true;
    }

    public void OnPressExitCreditsButton() {
        Utilities.TryGetComponent<Canvas>(creditsCanvas).sortingOrder = Constants.deactivatedCanvasSortOrder;
        isActivated = false;
        autoScroll = false;
    }

    private void StopScroll() {
        autoScroll = false;
    }

    private IEnumerator StartScrolling() {
        yield return new WaitForSeconds(pauseTime);
        autoScroll = true;
        startScrollCoroutine = null;
    }


    private void TryScroll() {
        if(!autoScroll) { return; }
        CalculateLowestPosition();
        if(lowestPosition > Screen.height*3/2) { MoveToInitialPosition(); }
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y+(scrollSpeed*Utilities.TryGetComponent<Canvas>(creditsCanvas).scaleFactor), transform.position.z), 0.5f);
    }


    private void CalculateLowestPosition() {
        Transform lastCredit = transform.GetChild(transform.childCount-1);
        lowestPosition = lastCredit.position.y - Utilities.TryGetComponent<TextMeshProUGUI>(lastCredit.gameObject).fontSize;
        Debug.Log(lowestPosition);
    }

    private void MoveToInitialPosition() {
        transform.position = new Vector3(Screen.width/2, -Screen.height/1.9f, 0);
    }
}
