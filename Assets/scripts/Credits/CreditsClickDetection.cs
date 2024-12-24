using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsClickDetection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    private CreditsGlobal creditsGlobal;

    void Awake() {
        creditsGlobal = FindFirstObjectByType<CreditsGlobal>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        creditsGlobal.OnPointerDown();
    }
    
    public void OnPointerUp(PointerEventData eventData) {
        creditsGlobal.OnPointerUp();
    }
}
