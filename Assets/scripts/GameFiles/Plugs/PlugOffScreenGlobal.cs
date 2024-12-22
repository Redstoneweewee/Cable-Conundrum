using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugOffScreenGlobal : MonoBehaviour {
    [SerializeField] [Range(0, 1)] private float lerpValue;
    // Update is called once per frame
    void Update() {
        PlugAttributes[] allPlugAttributes = FindObjectsByType<PlugAttributes>(FindObjectsSortMode.None);
        foreach(PlugAttributes plugAttribute in allPlugAttributes) {
            if(plugAttribute.isObstacle) { continue; }
            if(plugAttribute.isDragging) { continue; }

            TryMoveHorizontal(plugAttribute);
            TryMoveVertical(plugAttribute);
        }
    }

    private void TryMoveHorizontal(PlugAttributes plugAttribute) {
            Vector3 position = plugAttribute.transform.position;
            RectTransform rectTransform = Utilities.TryGetComponentInChildren<RectTransform>(plugAttribute.plugVisual);

            float leftMost = 0+(rectTransform.sizeDelta.x/2*LevelResizeGlobal.instance.finalScale);
            float rightMost = Screen.width-(rectTransform.sizeDelta.x/2*LevelResizeGlobal.instance.finalScale);
            if(position.x < leftMost) { 
                plugAttribute.transform.position = Vector3.Lerp(position, new Vector3(leftMost, position.y, 0), lerpValue); 
            }
            else if(position.x > rightMost) { 
                plugAttribute.transform.position = Vector3.Lerp(position, new Vector3(rightMost, position.y, 0), lerpValue); 
            }
    }

    private void TryMoveVertical(PlugAttributes plugAttribute) {
            Vector3 position = plugAttribute.transform.position;
            RectTransform rectTransform = Utilities.TryGetComponentInChildren<RectTransform>(plugAttribute.plugVisual);

            float bottomMost = 0+(rectTransform.sizeDelta.y/2*LevelResizeGlobal.instance.finalScale);
            float topMost = Screen.height-(rectTransform.sizeDelta.y/2*LevelResizeGlobal.instance.finalScale);
            if(position.y < bottomMost) { 
                plugAttribute.transform.position = Vector3.Lerp(position, new Vector3(position.x, bottomMost, 0), lerpValue); 
            }
            else if(position.y > topMost) { 
                plugAttribute.transform.position = Vector3.Lerp(position, new Vector3(position.x, topMost, 0), lerpValue); 
            }
    }
}
