using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoveRelativeToRectTransform : MonoBehaviour {
    private Vector2 cachedRectTransformSize;

    [SerializeField] private RectTransform relativeTo;
    [SerializeField] private ScreenAnchor anchor;
    [HideInInspector] private Vector2 anchorOffset;
    [SerializeField] private Vector2 offset;

    private ScreenAnchor cachedAnchor;
    private Vector2 cachedOffset;

    //-------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    //TODO - make a relative to size toggle for all MoveRelatives so that offsets change based on screen size
    //-------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    
    void Update() {
        if(!relativeTo) { return; }
        if(cachedAnchor != anchor || cachedRectTransformSize != new Vector2(relativeTo.sizeDelta.x, relativeTo.sizeDelta.y)) {
            UpdateAnchorOffset();
            cachedAnchor = anchor;
            UpdateOffset();
            cachedRectTransformSize = new Vector2(relativeTo.sizeDelta.x, relativeTo.sizeDelta.y);
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
                anchorOffset = new Vector2(relativeTo.position.x, relativeTo.position.y);
                break;
            case ScreenAnchor.Up:
                anchorOffset = new Vector2(relativeTo.position.x, relativeTo.position.y+relativeTo.sizeDelta.y/2);
                break;
            case ScreenAnchor.Down:
                anchorOffset = new Vector2(relativeTo.position.x, relativeTo.position.y-relativeTo.sizeDelta.y/2);
                break;
            case ScreenAnchor.Left:
                anchorOffset = new Vector2(relativeTo.position.x-relativeTo.sizeDelta.x/2, relativeTo.position.y);
                break;
            case ScreenAnchor.Right:
                anchorOffset = new Vector2(relativeTo.position.x+relativeTo.sizeDelta.x/2, relativeTo.position.y);
                break;
            case ScreenAnchor.TopLeft:
                anchorOffset = new Vector2(relativeTo.position.x-relativeTo.sizeDelta.x/2, relativeTo.position.y+relativeTo.sizeDelta.y/2);
                break;
            case ScreenAnchor.TopRight:
                anchorOffset = new Vector2(relativeTo.position.x+relativeTo.sizeDelta.x/2, relativeTo.position.y+relativeTo.sizeDelta.y/2);
                break;
            case ScreenAnchor.BottomLeft:
                anchorOffset = new Vector2(relativeTo.position.x-relativeTo.sizeDelta.x/2, relativeTo.position.y-relativeTo.sizeDelta.y/2);
                break;
            case ScreenAnchor.BottomRight:
                anchorOffset = new Vector2(relativeTo.position.x+relativeTo.sizeDelta.x/2, relativeTo.position.y-relativeTo.sizeDelta.y/2);
                break;
        }
    }
}
