using UnityEngine;

[ExecuteInEditMode]
public class ResizeCameraGlobal : MonoBehaviour {
    private Vector2 cachedScreenSize;

    void Update() {
        if(cachedScreenSize.x != Screen.width || cachedScreenSize.y != Screen.height) {
            ResizeCamera();
            cachedScreenSize = new Vector2(Screen.width, Screen.height);
        }
    }

    public void ResizeCamera() {
        float newSize = Screen.height/2;
        Camera.main.orthographicSize = newSize;
        transform.position = new Vector3(Screen.width/2, Screen.height/2, -10);
    }
}
