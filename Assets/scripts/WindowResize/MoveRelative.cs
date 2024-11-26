using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

[ExecuteInEditMode]
public class MoveRelative : RelativeBase {
    [SerializeField] private ScreenAnchor anchor;
                     private Vector2 anchorOffset;
    [SerializeField] private Vector2 offset;

    new public void Renew() {
        base.Renew();
        UpdateAnchorOffset();
        UpdateOffset();
    }

    private void UpdateOffset() {
        transform.position = new Vector3(anchorOffset.x+(offset.x*base.relativeScreenSize.x/base.absoluteScreenSize.x), anchorOffset.y+(offset.y*base.relativeScreenSize.y/base.absoluteScreenSize.y), 0);
    }

    private void UpdateAnchorOffset() {
        Vector2 objectSizeOffset = Vector2.zero;
        RectTransform rectTransform = GetComponentInChildren<RectTransform>(true);
        if(base.accountForObjectSize && rectTransform != null) { 
            objectSizeOffset = new Vector2(rectTransform.sizeDelta.x/2*rectTransform.localScale.x, rectTransform.sizeDelta.y/2*rectTransform.localScale.y); 
        }
        switch(anchor) {
            case ScreenAnchor.Center:
                anchorOffset = new Vector2(base.relativeToCenter.x, base.relativeToCenter.y);
                break;
            case ScreenAnchor.Up:
                anchorOffset = new Vector2(base.relativeToCenter.x, base.relativeToCenter.y+base.relativeToSize.y/2f-objectSizeOffset.y);
                break;
            case ScreenAnchor.Down:
                anchorOffset = new Vector2(base.relativeToCenter.x, base.relativeToCenter.y-base.relativeToSize.y/2f+objectSizeOffset.y);
                break;
            case ScreenAnchor.Left:
                anchorOffset = new Vector2(base.relativeToCenter.x-base.relativeToSize.x/2f+objectSizeOffset.x, base.relativeToCenter.y);
                break;
            case ScreenAnchor.Right:
                anchorOffset = new Vector2(base.relativeToCenter.x+base.relativeToSize.x/2f-objectSizeOffset.x, base.relativeToCenter.y);
                break;
            case ScreenAnchor.TopLeft:
                anchorOffset = new Vector2(base.relativeToCenter.x-base.relativeToSize.x/2f+objectSizeOffset.x, base.relativeToCenter.y+base.relativeToSize.y/2f-objectSizeOffset.y);
                break;
            case ScreenAnchor.TopRight:
                anchorOffset = new Vector2(base.relativeToCenter.x+base.relativeToSize.x/2f-objectSizeOffset.x, base.relativeToCenter.y+base.relativeToSize.y/2f-objectSizeOffset.y);
                break;
            case ScreenAnchor.BottomLeft:
                anchorOffset = new Vector2(base.relativeToCenter.x-base.relativeToSize.x/2f+objectSizeOffset.x, base.relativeToCenter.y-base.relativeToSize.y/2f+objectSizeOffset.y);
                break;
            case ScreenAnchor.BottomRight:
                anchorOffset = new Vector2(base.relativeToCenter.x+base.relativeToSize.x/2f-objectSizeOffset.x, base.relativeToCenter.y-base.relativeToSize.y/2f+objectSizeOffset.y);
                break;
            case ScreenAnchor.UpInverted:
                anchorOffset = new Vector2(base.relativeToCenter.x, base.relativeToCenter.y+base.relativeToSize.y/2f+objectSizeOffset.y);
                break;
            case ScreenAnchor.DownInverted:
                anchorOffset = new Vector2(base.relativeToCenter.x, base.relativeToCenter.y-base.relativeToSize.y/2f-objectSizeOffset.y);
                break;
            case ScreenAnchor.LeftInverted:
                anchorOffset = new Vector2(base.relativeToCenter.x-base.relativeToSize.x/2f-objectSizeOffset.x, base.relativeToCenter.y);
                break;
            case ScreenAnchor.RightInverted:
                anchorOffset = new Vector2(base.relativeToCenter.x+base.relativeToSize.x/2f+objectSizeOffset.x, base.relativeToCenter.y);
                break;
        }
    }
}
