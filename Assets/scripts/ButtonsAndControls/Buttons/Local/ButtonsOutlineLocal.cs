using System.Collections;
using System.Collections.Generic;
using Nobi.UiRoundedCorners;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonsOutlineLocal : MonoBehaviour {
    [SerializeField]          private GameObject outline;
    [SerializeField] [Min(0)] private float width = 10f;

    private ImageWithRoundedCorners imageWithRoundedCorners;
    private RectTransform           rectTransform;

    private Vector3 cachedPosition = Vector2.zero;
    private float   cachedWidth    = 0f;
    private Vector2 cachedSize     = Vector2.zero;
    private float   cachedRadius   = 0f;

    void Awake() {
        imageWithRoundedCorners = Utilities.TryGetComponent<ImageWithRoundedCorners>(gameObject);
        rectTransform = Utilities.TryGetComponent<RectTransform>(gameObject);
    }

    void Update() {
        Renew();
    }

    public void Renew() {
        imageWithRoundedCorners = Utilities.TryGetComponent<ImageWithRoundedCorners>(gameObject);
        rectTransform = Utilities.TryGetComponent<RectTransform>(gameObject);
        if(cachedWidth != width || rectTransform.sizeDelta != cachedSize || cachedRadius != imageWithRoundedCorners.radius || cachedPosition != rectTransform.position) {
            cachedWidth = width;
            cachedSize = rectTransform.sizeDelta;
            cachedRadius = imageWithRoundedCorners.radius;
            cachedPosition = rectTransform.position;
            RenewOutline();
        }
    }

    private void RenewOutline() {
        RectTransform outlineRectTransform = Utilities.TryGetComponent<RectTransform>(outline);
        ImageWithRoundedCorners outlineCorners = Utilities.TryGetComponent<ImageWithRoundedCorners>(outline);
        outlineRectTransform.sizeDelta = new Vector2(cachedSize.x + cachedWidth*2, cachedSize.y + cachedWidth*2);
        float radiusRatio = cachedRadius / cachedSize.y;
        outlineCorners.radius = outlineRectTransform.sizeDelta.y * radiusRatio;
        outlineRectTransform.position = cachedPosition;
        outlineCorners.Refresh();
    }
}
