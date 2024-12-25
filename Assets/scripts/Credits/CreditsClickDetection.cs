using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsClickDetection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public void OnPointerDown(PointerEventData eventData) {
        //CreditsGlobal.Instance.OnPointerDown();
    }
    
    public void OnPointerUp(PointerEventData eventData) {
        //CreditsGlobal.Instance.OnPointerUp();
    }
}
