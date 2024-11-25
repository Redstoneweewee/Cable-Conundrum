using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RelativeBase : MonoBehaviour {


    [HideInInspector] public Vector2 cachedScreenSize;
    
    [Tooltip("A lower renew index will renew before a higher renew index.")]
    [SerializeField] [Min(0)] public int renewIndex = 0;

    [Tooltip("RelativeResize will resize the offset based on relative size/absolute size.")]
    /* 
      absolute resize (default): offset does not change depending on screen size (slope = 0)
     |
     |<---------------->
     |
     |
     |------------------
    
      relative resize: slope is (rel size/abs size), function always ends at (0,0)
     |                /
     |            /
     |        /
     |    /
     |/
     -----------------

     Link to Desmos Graph example: https://www.desmos.com/calculator/bmhajrljys 
    */
    [SerializeField]  public bool relativeResize = false;
    [SerializeField]  public bool accountForObjectSize = false;
    [HideInInspector] public Vector2 absoluteScreenSize = new Vector2(1920, 1080);
    [HideInInspector] public Vector2 relativeScreenSize;
    [Tooltip("If relativeTo is null, will default to the current screen size.")]
    [SerializeField]  public RectTransform relativeTo;
    [HideInInspector] public Vector2 relativeToSize;
    [HideInInspector] public Vector2 relativeToCenter;

    public void Renew() {
        Debug.Log("Renewed "+transform.name);
        absoluteScreenSize = new Vector2(1920, 1080);
        if(relativeResize) { relativeScreenSize = new Vector2(Screen.width, Screen.height); }
        else { relativeScreenSize = new Vector2(1920, 1080); }
        
        if(!relativeTo) {
            relativeToSize = new Vector2(Screen.width, Screen.height);
            relativeToCenter = new Vector2(Screen.width/2f, Screen.height/2f);
        }
        else {
            relativeToSize = relativeTo.sizeDelta*relativeTo.localScale;
            relativeToCenter = relativeTo.position;
        }
        cachedScreenSize = new Vector2(Screen.width, Screen.height);
    }
}
