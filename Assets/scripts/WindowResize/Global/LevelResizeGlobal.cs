using System.Collections;
using UnityEngine;


[HideInInspector] 
public class LevelResizeGlobal : Singleton<LevelResizeGlobal> {

    [SerializeField] private float cachedScaleFactor = 1;
    [SerializeField] private float scaleFactor       = 1;
    [SerializeField] public Canvas gameCanvas;
    [SerializeField] public float scaleMultiplier = 1;

    [SerializeField] public float finalScale;
    
    //Game distances and offsets normal values (1920 x 1080)
    //[HideInInspector] private static float   staticSocketSize = 133;
    //[HideInInspector] private static Vector2 staticPowerSwitchSize = new Vector2(140, 38);
    //[HideInInspector] private static Vector2 staticEyesSize        = new Vector2(140, 140);
    //[HideInInspector] private static Vector2 static1x1PlugSize     = new Vector2(140, 140);
    //[HideInInspector] private static Vector2 static1x3PlugSize     = new Vector2(311, 140);
    //[HideInInspector] private static Vector2 static3x1PlugSize     = new Vector2(140, 311);
    //[HideInInspector] private static Vector2 static3x1v2PlugSize   = new Vector2(160, 311);
    //[HideInInspector] private static Vector2 static3x3LPlugSize    = new Vector2(311, 311);



    [HideInInspector] private static float staticPlugLockingDistance = 28;
    [HideInInspector] private static float staticStandardSpriteSize = 140;
    [HideInInspector] private static float staticCableWidith = 26;
    //[HideInInspector] private static float staticRotationAndStraightCableOverlap = 13;

    [HideInInspector] public  static float staticElectricalStripSeparatorDistance = 30;
    [HideInInspector] public  static Vector2 staticElectricalStripBaseSize = new Vector2(staticStandardSpriteSize, staticStandardSpriteSize);
    [HideInInspector] private static float   staticJointDistance       = (staticElectricalStripBaseSize.x + staticElectricalStripSeparatorDistance)/2; // = 85
    [HideInInspector] public  static Vector2 staticPowerSwitchBaseSize = new Vector2(staticStandardSpriteSize, 38);
    [HideInInspector] private static Vector2 staticRotationCableSize = new Vector2(staticCableWidith, staticCableWidith);
    [HideInInspector] private static Vector2 staticStraightCableSize = new Vector2(staticCableWidith, staticJointDistance);

    [HideInInspector] private static float staticTableSnapDistance = staticJointDistance/2;
    [HideInInspector] private static float   staticTableTopDistanceFromLeg = 18;
    [HideInInspector] private static Vector2 staticStartingPlugOffset = new Vector2(staticJointDistance*1.25f, staticJointDistance*1.25f);
    [HideInInspector] private static float staticStartingPlugOffsetRightSideAdd = 1f;

    
    //Game distances and offsets
    [HideInInspector] public float   plugLockingDistance;
    [HideInInspector] public float   standardSpriteSize;
    [HideInInspector] public float   cableWidith;
    //[HideInInspector] public float   rotationAndStraightCableOverlap;

    [HideInInspector] public float   electricalStripSeparatorDistance;
    [HideInInspector] public Vector2 electricalStripBaseSize;
    [HideInInspector] public float   jointDistance;
    [HideInInspector] public Vector2 powerSwitchBaseSize;
    [HideInInspector] public Vector2 rotationCableSize;
    [HideInInspector] public Vector2 straightCableSize;

    [HideInInspector] public float   tableSnapDistance;
    [HideInInspector] public float   tableTopDistanceFromLeg;
    [HideInInspector] public Vector2 startingPlugOffset;
    [HideInInspector] public float   startingPlugOffsetRightSideAdd;

    public override IEnumerator Initialize() {
        RenewValues();
        yield return null;
    }
    
 
    void Update() {
        if(!GameObject.FindGameObjectWithTag("GameCanvas")) { return; }
        gameCanvas = Utilities.TryGetComponent<Canvas>(GameObject.FindGameObjectWithTag("GameCanvas"));
        if(gameCanvas && gameCanvas.scaleFactor != cachedScaleFactor) { 
            Debug.Log("renew");
            RenewValues();
            cachedScaleFactor = gameCanvas.scaleFactor;
        }
    }

    void RenewValues() {
        scaleFactor = 1;
        if(gameCanvas) { scaleFactor = gameCanvas.scaleFactor; }
        finalScale = scaleFactor*scaleMultiplier;
        
        plugLockingDistance               = staticPlugLockingDistance             *finalScale;
        //rotationAndStraightCableOverlap = staticRotationAndStraightCableOverlap *finalScale;
        electricalStripSeparatorDistance  = staticElectricalStripSeparatorDistance*finalScale;
        jointDistance                     = staticJointDistance                   *finalScale;
        tableSnapDistance                 = staticTableSnapDistance               *finalScale;
        tableTopDistanceFromLeg           = staticTableTopDistanceFromLeg         *finalScale;
        startingPlugOffset                = staticStartingPlugOffset              *finalScale;
        startingPlugOffsetRightSideAdd    = staticStartingPlugOffsetRightSideAdd  *finalScale;
        powerSwitchBaseSize               = staticPowerSwitchBaseSize             *finalScale;
        electricalStripBaseSize           = staticElectricalStripBaseSize         *finalScale;
        
        rotationCableSize                 = staticRotationCableSize;
        straightCableSize                 = staticStraightCableSize;
        standardSpriteSize                = staticStandardSpriteSize;
        cableWidith                       = staticCableWidith;
        //Debug.Log($"powerSwitchBaseSize: {powerSwitchBaseSize}");
        //Debug.Log($"electricalStripSeparatorDistance: {electricalStripSeparatorDistance}");
        //TODO - call all calculations to recalculate offsets, distances, and sizes
        //including resizing plug, sockets, etc. sizes
        GridsController.Instance?.InitializeOld();
        if(WinningMessageSizeGlobal.Instance != null) { WinningMessageSizeGlobal.Instance.reinitialize = true; }
    }
}
