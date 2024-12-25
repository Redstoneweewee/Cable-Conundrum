using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class WinningMessageSizeGlobal : Singleton<WinningMessageSizeGlobal> {
    [SerializeField] public  bool reinitialize = false;
    [SerializeField] private float offset = 45f;
    [SerializeField] public GameObject background;
    [SerializeField] public GameObject text;
    [SerializeField] public GameObject button;
                     private RectTransform backgroundRectTransform;
                     private RectTransform buttonRectTransform;

    private float numSize = 67;
    private float initialTextSize = 904;

    [SerializeField] private float backgroundHeight = 180;

    private int cachedNumberOfNumbers;
    //Each number adds ~67 pixels to text
    //One-numbered text is 971 pixels
    //Two-numbered text is 1038 pixels
    //etc.

    public override void OnAwake() {
        Initialize();
    }

    private void Initialize() {
        backgroundRectTransform = Utilities.TryGetComponent<RectTransform>(background);
        buttonRectTransform     = Utilities.TryGetComponentInChildren<RectTransform>(button);
    }

    void Update() {
        if(reinitialize) {
            Initialize();
        }
        if(cachedNumberOfNumbers != CountNumberOfNumbers(Utilities.GetText(text)) || reinitialize) {
            
            float textSize = initialTextSize + CountNumberOfNumbers(Utilities.GetText(text)) * numSize;
            cachedNumberOfNumbers = CountNumberOfNumbers(Utilities.GetText(text));
            backgroundRectTransform.sizeDelta = new Vector2((offset*3) + buttonRectTransform.sizeDelta.x + textSize,backgroundHeight);

            button.transform.position = new Vector2(background.transform.position.x + backgroundRectTransform.sizeDelta.x/2*LevelResizeGlobal.Instance.finalScale - buttonRectTransform.sizeDelta.x/2*LevelResizeGlobal.Instance.finalScale - offset*LevelResizeGlobal.Instance.finalScale, background.transform.position.y);
            text.transform.position = new Vector2(button.transform.position.x - textSize/2*LevelResizeGlobal.Instance.finalScale - buttonRectTransform.sizeDelta.x/2*LevelResizeGlobal.Instance.finalScale - offset*LevelResizeGlobal.Instance.finalScale, button.transform.position.y);
            
            reinitialize = false;
        }
    }

    private int CountNumberOfNumbers(string text) {
        int output = 0;
        foreach(char c in text) {
            if(Char.IsDigit(c)) { output++; }
        }
        return output;
    }
}
