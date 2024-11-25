using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SizeRelative : RelativeBase {

    [SerializeField] private Vector2 sizeReduction;
    [SerializeField] private Vector2 offset;

    void Awake() {
        Renew();
    }
    
    new void Update() {
        base.accountForObjectSize = true;
        base.Update();
        if(Application.isPlaying && base.cachedScreenSize != new Vector2(Screen.width, Screen.height)) {
            Renew();
        }
    }

    new public void Renew() {
        base.Renew();
        UpdateSize();
    }
    
    private void UpdateSize() {
        RectTransform rectTransform = GetComponentInChildren<RectTransform>();
        if(rectTransform) {
            rectTransform.position  = new Vector3(base.relativeToCenter.x+offset.x, base.relativeToCenter.y+offset.y, 0);
            rectTransform.sizeDelta = new Vector2(base.relativeToSize.x-(sizeReduction.x*base.relativeScreenSize.x/base.absoluteScreenSize.x),
                                                  base.relativeToSize.y-(sizeReduction.y*base.relativeScreenSize.y/base.absoluteScreenSize.y));
        }
    }
}

