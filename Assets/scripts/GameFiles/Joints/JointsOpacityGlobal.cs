using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class JointsOpacityGlobal : Singleton<JointsOpacityGlobal> {
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

    public override IEnumerator Initialize() {
        yield return null;
    }
    public override void OnAwake() { }
    
    void Start() {
        ResetAllVariables();
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

        if(JointsData.Instance.jointsEnabled) { tf = t; }
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

        if(!JointsData.Instance.jointsEnabled) { 
            //td = t; 
            c41 = f31(t);
            c42 = f32(t);
            CalculateDValues();
        }

        DebugC.Instance?.LogMath($"p: {p}");
        DebugC.Instance?.LogMath($"t: {t}");
        DebugC.Instance?.LogMath($"tf: {tf}");
        DebugC.Instance?.LogMath($"a3: {a3}");
        DebugC.Instance?.LogMath($"b31: {b31}");
        DebugC.Instance?.LogMath($"b32: {b32}");

        DebugC.Instance?.LogMath($"c41: {c41}");
        DebugC.Instance?.LogMath($"c42: {c42}");
        DebugC.Instance?.LogMath($"d41: {d41}");
        DebugC.Instance?.LogMath($"d42: {d42}");
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
        bool jointsEnabled = JointsData.Instance.jointsEnabled;
        ModifyAllVariables();
        float opacityNum;

        if(jointsEnabled && isFirstOpacity) { 
            if(previousIsFirstOpacity) { opacityNum = f1(t, d41); DebugC.Instance?.LogMath("case 11, f11(t): "+f1(t, d41)); }
            else                       { opacityNum = f1(t, d42); DebugC.Instance?.LogMath("case 12, f12(t): "+f1(t, d42)); }
        }
        else if(jointsEnabled && !isFirstOpacity) { 
            if(previousIsFirstOpacity) { opacityNum = f2(t, d41); DebugC.Instance?.LogMath("case 21, f21(t): "+f2(t, d41));  }
            else                       { opacityNum = f2(t, d42); DebugC.Instance?.LogMath("case 22, f22(t): "+f2(t, d42));  }
            
        }
        else { 
            if(isFirstOpacity) { opacityNum = f31(t); DebugC.Instance?.LogMath("case 31, f31(t): "+f31(t)); }
            else               { opacityNum = f32(t); DebugC.Instance?.LogMath("case 32, f32(t): "+f32(t)); }
            
        }
        DebugC.Instance?.LogMath("isFirstLoop: "+isFirstLoop);
        DebugC.Instance?.LogMath("isFirstOpacity: "+isFirstOpacity);
        DebugC.Instance?.LogMath("previousIsFirstOpacity: "+previousIsFirstOpacity);
        DebugC.Instance?.LogMath("opacityNum: "+opacityNum);
        float newOpacity;
        newOpacity = math.remap(0f, 1f, Constants.joinOpacityMin, Constants.joinOpacityMax, opacityNum);
        //Debug.LogMath("opacityNum: "+opacityNum);
        //Debug.LogMath("newOpacity: "+newOpacity);
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
