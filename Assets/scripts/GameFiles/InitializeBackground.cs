using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class InitializeBackground : Singleton<InitializeBackground> {
    RectTransform rectangularTransform;

    public override void OnAwake() { }
    
    void Start() {
        rectangularTransform = Utilities.TryGetComponent<RectTransform>(gameObject);
    }

    // Start is called before the first frame update
    void Update() {
        if(rectangularTransform.sizeDelta != new Vector2(Screen.width, Screen.height)) {
            rectangularTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}