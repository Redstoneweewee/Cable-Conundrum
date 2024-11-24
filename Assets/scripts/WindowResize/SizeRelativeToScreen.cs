using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SizeRelativeToScreen : MonoBehaviour {
    private Vector2 cachedScreenSize;

    [SerializeField] private Vector2 sizeReduction;
    [SerializeField] private Vector2 offset;

    private Vector2 cachedSizeReduction;
    private Vector2 cachedOffset;

    void Update() {
        if(cachedOffset != offset || cachedScreenSize != new Vector2(Screen.width, Screen.height) || cachedSizeReduction != sizeReduction) {
            UpdateSize();
            cachedOffset = offset;
            cachedSizeReduction = sizeReduction;
            cachedScreenSize = new Vector2(Screen.width, Screen.height);
        }
    }
    
    private void UpdateSize() {
        RectTransform rectTransform = GetComponentInChildren<RectTransform>();
        if(rectTransform) {
            rectTransform.position = new Vector3(Screen.width/2+offset.x, Screen.height/2+offset.y, 0);
            rectTransform.sizeDelta = new Vector2((1920-sizeReduction.x)/1920*Screen.width, (1080-sizeReduction.y)/1080*Screen.height);
        }
    }
}

