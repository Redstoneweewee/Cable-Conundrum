using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeCameraGlobal : WindowSizeChange<ResizeCameraGlobal> {

    public override IEnumerator Initialize() {
        yield return null;
    }
    
    void Update() {
        if(!ScriptInitializationGlobal.Instance.ShouldUpdate) { return; }
        if(base.GetScreenSizeChange()) { ResizeCamera(); }
    }

    public void ResizeCamera() {
        float newSize = Screen.height/2;
        Utilities.TryGetComponent<Camera>(gameObject).orthographicSize = newSize;
        transform.position = new Vector3(Screen.width/2, Screen.height/2, -10);
    }
}
