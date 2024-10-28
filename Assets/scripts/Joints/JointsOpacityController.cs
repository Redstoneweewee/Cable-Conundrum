using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class JointsOpacityController : MonoBehaviour {
    public DebugC DebugC { get ; set; }
    private JointsController jointsController;
    private IEnumerator opacityCoroutine;
    [SerializeField] private Material jointMaterial;
    [SerializeField] private float opacityLoopPeriod = 1;
    //[SerializeField] [Range(0.01f, 10f)]private float decaySpeed = 1;
    private float p = 0;  //period of each loop; is equal to opacityLoopPeriod
    private float t = 0;  //time since the loop started
    private float a3 = 0; //coefficient of opacity decay (f3)
    private float b31 = 0; //x-offset of opacity decay for first opacity (f3)
    private float b32 = 0; //x-offset of opacity decay for !first opacity (f3)
    private float tf = 0; //the end-time (when joints get disabled)

    //private float td = 0;  //delta time after joints get disabled until opacity reaches 0 (must always be in range [xf, b3])
    //public  float tfd = 0;  //time elapsed since the start of the new opacity loop (changes every time the player clicks onto a cable)
    private float c41 = 0;  //height of opacity decay for first opacity at delta time
    private float c42 = 0;  //height of opacity decay for !first opacity at delta time
    private float d41 = 0; //x-offset for first opacity if used (f1)
    private float d42 = 0; //x-offset for !first opacity if used (f2)
    private bool previousIsFirstOpacity = true;
    private bool isFirstOpacity = true;
    private bool isFirstLoop = true;


    public IEnumerator OpacityCoroutine {get{return opacityCoroutine;} set{opacityCoroutine = value;}}
    public bool PreviousIsFirstOpacity  {get{return previousIsFirstOpacity;}   set{previousIsFirstOpacity   = value;}}
    public bool        IsFirstOpacity   {get{return isFirstOpacity;}   set{isFirstOpacity   = value;}}
    public bool        IsFirstLoop   {get{return isFirstLoop;}   set{isFirstLoop   = value;}}

    // Start is called before the first frame update
    void Start() {
        DebugC = DebugC.Get();
        jointsController = GetComponent<JointsController>();
        ResetAllVariables();
        //Debug.Log(math.sin((2*math.PI/p)*(0.5+d41) - math.PI/2));
    }


    private void ResetAllVariables() {
        p = opacityLoopPeriod;
        t = 0;
        tf = 0;
        a3 = math.PI/p*math.cos(math.PI);
        b31 = 0;
        b32 = 0;

        //td = 0;
        c41 = 0;
        c42 = 0;
        d41 = 0;
        d42 = 0;
    }

    private void ModifyAllVariables() {
        p = opacityLoopPeriod;
        t += 0.01f;
        //tfd += 0.01f;

        if(jointsController.JointsEnabled) { tf = t; }
        a3 = math.PI/p*math.cos(math.PI);
        if(previousIsFirstOpacity) { 
            if(isFirstLoop) {
                b31 = -f1(tf, 0)/a3 + tf;
                b32 = -f2(tf, 0)/a3 + tf;
            }
            else {
                b31 = -f1(tf, 0)/a3 + tf;
                b32 = -f2(tf, 0)/a3 + tf;
            }
        }
        else { 
            if(isFirstLoop) {
                b31 = -f1(tf, 0)/a3 + tf;
                b32 = -f2(tf, 0)/a3 + tf;
            }
            else {
                b31 = -f1(tf, 0)/a3 + tf;
                b32 = -f2(tf, 0)/a3 + tf;
            }
        }

        if(!jointsController.JointsEnabled) { 
            //td = t; 
            c41 = f31(t);
            c42 = f32(t);
            CalculateDValues();
        }

        DebugC.Log($"p: {p}");
        DebugC.Log($"t: {t}");
        DebugC.Log($"tf: {tf}");
        DebugC.Log($"a3: {a3}");
        DebugC.Log($"b31: {b31}");
        DebugC.Log($"b32: {b32}");

        ////Debug.Log($"td: {td}");
        DebugC.Log($"c41: {c41}");
        DebugC.Log($"c42: {c42}");
        DebugC.Log($"d41: {d41}");
        DebugC.Log($"d42: {d42}");
    }

    public void CalculateDValues() {
        d41 = p*math.asin(2*c41 - 1)/(2*math.PI) + p/4 - t;
        d42 = p*math.asin(2*c42 - 1)/(2*math.PI) + p/4 - t;
        ////if(float.IsNaN(d42)) { d42 = 0; }
    }

    //first opacity wave
    //returns the opacity value with range [Constants.joinOpacityMin, Constants.joinOpacityMax]
    private float f1(float x, float offset) {
        return math.sin((2*math.PI/p)*(x+offset) - math.PI/2)/2f + 1/2f;
    }

    //!first opacity wave
    //returns the opacity value with range [Constants.joinOpacityMid, Constants.joinOpacityMax]
    private float f2(float x, float offset) {
        return math.sin((2*math.PI/p)*(x+offset) - math.PI/2)*((1-Constants.joinOpacityMid)/2f) + (1 - (1-Constants.joinOpacityMid)/2f);
    }


    //opacity decay for first opacity
    //returns the opacity value as it decays linearly with a slope of max(f1'(x))
    private float f31(float x) {
        float output = a3*(x - b31);
        if(output < 0) { output = 0;}
        return output;
    }


    //opacity decay for !first opacity
    //returns the opacity value as it decays linearly with a slope of max(f1'(x))
    private float f32(float x) {
        float output = a3*(x - b32);
        if(output < 0) { output = 0;}
        return output;
    }

    
    public IEnumerator ModifyJointsOpacity() {
        yield return new WaitForSeconds(0.01f);
        bool jointsEnabled = jointsController.JointsEnabled;
        ModifyAllVariables();
        float opacityNum;

        if(jointsEnabled && isFirstOpacity) { 
            if(previousIsFirstOpacity) { opacityNum = f1(t, d41); DebugC.Log("case 11, f11(t): "+f1(t, d41)); }
            else                       { opacityNum = f1(t, d42); DebugC.Log("case 12, f12(t): "+f1(t, d42)); }
        }
        else if(jointsEnabled && !isFirstOpacity) { 
            if(previousIsFirstOpacity) { opacityNum = f2(t, d41); DebugC.Log("case 21, f21(t): "+f2(t, d41));  }
            else                       { opacityNum = f2(t, d42); DebugC.Log("case 22, f22(t): "+f2(t, d42));  }
            
        }
        else { 
            if(isFirstOpacity) { opacityNum = f31(t); DebugC.Log("case 31, f31(t): "+f31(t)); }
            else               { opacityNum = f32(t); DebugC.Log("case 32, f32(t): "+f32(t)); }
            
        }
        DebugC.Log("isFirstLoop: "+isFirstLoop);
        DebugC.Log("isFirstOpacity: "+isFirstOpacity);
        DebugC.Log("previousIsFirstOpacity: "+previousIsFirstOpacity);
        DebugC.Log("opacityNum: "+opacityNum);
        float newOpacity;
        newOpacity = math.remap(0f, 1f, Constants.joinOpacityMin, Constants.joinOpacityMax, opacityNum);
        //Debug.Log("opacityNum: "+opacityNum);
        //Debug.Log("newOpacity: "+newOpacity);
                //jointImage.color = new Color(jointImage.color.r, jointImage.color.g, jointImage.color.b, newOpacity);
        //jointMaterial.color = new Color(jointMaterial.color.r, jointMaterial.color.g, jointMaterial.color.b, newOpacity);
        jointMaterial.color = new Color(jointMaterial.color.r, jointMaterial.color.g, jointMaterial.color.b, newOpacity);
        
        //if(!jointsEnabled && opacityNum < 0.5) {
        //    isFirstOpacity = true;
        //}
        if(opacityNum >= 0.9f) {
            isFirstOpacity = false;
        }


        if(!jointsEnabled && opacityNum == 0) {
            ResetAllVariables();
            if(opacityCoroutine != null) { StopCoroutine(opacityCoroutine); }
            opacityCoroutine = null;
            isFirstLoop = true;
        }
        else { 
            opacityCoroutine = ModifyJointsOpacity();
            StartCoroutine(opacityCoroutine);
        }
    }
}
