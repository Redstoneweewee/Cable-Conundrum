using UnityEngine;

[ExecuteInEditMode]
public class ResizeCameraGlobal : WindowSizeChange {

    void Update() {
        if(base.GetScreenSizeChange()) { ResizeCamera(); }
    }

    public void ResizeCamera() {
        float newSize = Screen.height/2;
        Camera.main.orthographicSize = newSize;
        transform.position = new Vector3(Screen.width/2, Screen.height/2, -10);
    }
}
