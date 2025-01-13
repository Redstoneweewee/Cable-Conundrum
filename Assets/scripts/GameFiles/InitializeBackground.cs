using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class InitializeBackground : Singleton<InitializeBackground> {
    RectTransform rectangularTransform;

    public override IEnumerator Initialize() {
        rectangularTransform = Utilities.TryGetComponent<RectTransform>(gameObject);
        yield return null;
    }

    // Start is called before the first frame update
    void Update() {
        if(!ScriptInitializationGlobal.Instance.ShouldUpdate) { return; }
        if(rectangularTransform.sizeDelta != new Vector2(Screen.width, Screen.height)) {
            rectangularTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}