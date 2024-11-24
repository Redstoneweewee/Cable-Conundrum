using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoveRelativeToScreen : MonoBehaviour {
    private Vector2 cachedScreenSize;

    [SerializeField] private ScreenAnchor anchor;
    [HideInInspector] private Vector2 anchorOffset;
    [SerializeField] private Vector2 offset;

    private ScreenAnchor cachedAnchor;
    private Vector2 cachedOffset;
    // Update is called once per frame
    
    
    void Update() {
        if(cachedAnchor != anchor || cachedScreenSize != new Vector2(Screen.width, Screen.height)) {
            UpdateAnchorOffset();
            cachedAnchor = anchor;
            UpdateOffset();
            cachedScreenSize = new Vector2(Screen.width, Screen.height);
        }
        if(cachedOffset != offset) {
            UpdateOffset();
            cachedOffset = offset;
        }
    }
    
    

    private void UpdateOffset() {
        transform.position = new Vector3(anchorOffset.x+offset.x, anchorOffset.y+offset.y, 0);
    }

    private void UpdateAnchorOffset() {
        switch(anchor) {
            case ScreenAnchor.Center:
                anchorOffset = new Vector2(Screen.width/2, Screen.height/2);
                break;
            case ScreenAnchor.Up:
                anchorOffset = new Vector2(Screen.width/2, Screen.height);
                break;
            case ScreenAnchor.Down:
                anchorOffset = new Vector2(Screen.width/2, 0);
                break;
            case ScreenAnchor.Left:
                anchorOffset = new Vector2(0, Screen.height/2);
                break;
            case ScreenAnchor.Right:
                anchorOffset = new Vector2(Screen.width, Screen.height/2);
                break;
            case ScreenAnchor.TopLeft:
                anchorOffset = new Vector2(0, Screen.height);
                break;
            case ScreenAnchor.TopRight:
                anchorOffset = new Vector2(Screen.width, Screen.height);
                break;
            case ScreenAnchor.BottomLeft:
                anchorOffset = new Vector2(0, 0);
                break;
            case ScreenAnchor.BottomRight:
                anchorOffset = new Vector2(Screen.width, 0);
                break;
        }
    }
}
