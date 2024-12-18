using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowSizeChange : MonoBehaviour {
    private Vector2 cachedScreenSize;

    public bool GetScreenSizeChange() {
        if(Application.isPlaying && cachedScreenSize != new Vector2(Screen.width, Screen.height)) {
            cachedScreenSize = new Vector2(Screen.width, Screen.height);
            return true;
        }
        return false;
    }
}