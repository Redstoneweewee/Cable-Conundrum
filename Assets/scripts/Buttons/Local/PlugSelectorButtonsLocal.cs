using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlugSelectorButtonsLocal : MonoBehaviour, IPointerDownHandler {
    [HideInInspector] public int id;

    public void OnPointerDown(PointerEventData eventData) {
        Utilities.TryGetComponentInParent<PlugSelectorController>(gameObject).OnClickPlugSelectorButton(id);
    }
}
