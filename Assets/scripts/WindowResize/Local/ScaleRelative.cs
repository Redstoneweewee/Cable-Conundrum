using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleRelative : RelativeBase {
    [SerializeField] private float scaleMultiplier = 0.6f;
    [SerializeField] private ScaleTypes scaleType = ScaleTypes.Either;

    void Update() {
        base.relativeResize = true;
        base.accountForObjectSize = true;
    }

    new public void Renew() {
        base.Renew();
        base.accountForObjectSize = true;
        UpdateSize();
    }
    
    private void UpdateSize() {
        RectTransform rectTransform = GetComponentInChildren<RectTransform>();
        if(rectTransform) {
            if(scaleType == ScaleTypes.Horizontal) {
                rectTransform.localScale = new Vector2(scaleMultiplier*base.relativeScreenSize.x/base.absoluteScreenSize.x + 1 - scaleMultiplier,
                                                       scaleMultiplier*base.relativeScreenSize.x/base.absoluteScreenSize.x + 1 - scaleMultiplier);
            }
            else if(scaleType == ScaleTypes.Vertical) {
                rectTransform.localScale = new Vector2(scaleMultiplier*base.relativeScreenSize.y/base.absoluteScreenSize.y + 1 - scaleMultiplier,
                                                       scaleMultiplier*base.relativeScreenSize.y/base.absoluteScreenSize.y + 1 - scaleMultiplier);
            }
            else if(scaleType == ScaleTypes.Both) { //Scales based on both values
                rectTransform.localScale = new Vector2(scaleMultiplier*base.relativeScreenSize.x/base.absoluteScreenSize.x + 1 - scaleMultiplier,
                                                       scaleMultiplier*base.relativeScreenSize.y/base.absoluteScreenSize.y + 1 - scaleMultiplier);
            }
            else if(scaleType == ScaleTypes.Either) { //Scales based on the smaller value
                float relativeSize = base.relativeScreenSize.x;
                float absoluteSize = base.absoluteScreenSize.x;
                if(base.relativeScreenSize.x/base.absoluteScreenSize.x > base.relativeScreenSize.y/base.absoluteScreenSize.y) { 
                    relativeSize = base.relativeScreenSize.y;
                    absoluteSize = base.absoluteScreenSize.y;
                }
                rectTransform.localScale = new Vector2(scaleMultiplier*relativeSize/absoluteSize + 1 - scaleMultiplier,
                                                       scaleMultiplier*relativeSize/absoluteSize + 1 - scaleMultiplier);
            }
            else if(scaleType == ScaleTypes.KeepWithScreen) {
                Debug.Log($"Screen size: {Screen.width}, {Screen.height}");
                Vector2 normalSize = rectTransform.sizeDelta;
                Vector2 sizeRatio = new Vector2(normalSize.x/Screen.width, normalSize.y/Screen.height);
                float scale = 1/sizeRatio.x;
                if(sizeRatio.x > sizeRatio.y) {
                    scale = 1/sizeRatio.y;
                }
                rectTransform.localScale = new Vector2(scale*scaleMultiplier, scale*scaleMultiplier);
            }
        }
    }
}
