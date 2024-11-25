using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleRelative : RelativeBase {
    [SerializeField] private float scaleMultiplier = 0.6f;
    [SerializeField] private ScaleTypes scaleType = ScaleTypes.Either;

    void Awake() {
        Renew();
    }
    
    new void Update() {
        base.relativeResize = true;
        base.accountForObjectSize = true;
        base.Update();
        if(Application.isPlaying && base.cachedScreenSize != new Vector2(Screen.width, Screen.height)) {
            Renew();
        }
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
                Debug.Log(scaleMultiplier);
                float relativeSize = base.relativeScreenSize.x;
                float absoluteSize = base.absoluteScreenSize.x;
                if(base.relativeScreenSize.x/base.absoluteScreenSize.x > base.relativeScreenSize.y/base.absoluteScreenSize.y) { 
                    relativeSize = base.relativeScreenSize.y;
                    absoluteSize = base.absoluteScreenSize.y;
                }
                rectTransform.localScale = new Vector2(scaleMultiplier*relativeSize/absoluteSize + 1 - scaleMultiplier,
                                                       scaleMultiplier*relativeSize/absoluteSize + 1 - scaleMultiplier);
            }
        }
    }
}
