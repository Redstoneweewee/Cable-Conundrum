using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonId : MonoBehaviour, IPointerDownHandler {
    public int id;

    public void OnPointerDown(PointerEventData eventData) {
        GetComponentInParent<PlugSelectorController>().OnClickPlugSelectorButton(id);
    }
}
