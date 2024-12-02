using UnityEngine;


[HideInInspector] public class LevelResizeGlobal : MonoBehaviour {

    public static LevelResizeGlobal instance;
    [SerializeField] public float scaleMultiplier = 1;
    [Tooltip("Just for seeing, cannot be changed.")]
    [SerializeField] public float levelScale;

    
    //Game distances and offsets normal values (1920 x 1080)
    [HideInInspector] private static float staticPlugLockingDistance = 28;
    [HideInInspector] private static float staticStandardSpriteSize = 140;
    [HideInInspector] private static float staticCableWidith = 26;
    [HideInInspector] private static float staticRotationAndStraightCableOverlap = 13;

    [HideInInspector] private static float staticElectricalStripSeparatorSize = 30;
    [HideInInspector] private static Vector2 staticElectricalStripBaseSize = new Vector2(staticStandardSpriteSize, staticStandardSpriteSize);
    [HideInInspector] private static float   staticJointDistance       = (staticElectricalStripBaseSize.x + staticElectricalStripSeparatorSize)/2; // = 85
    [HideInInspector] private static Vector2 staticPowerSwitchBaseSize = new Vector2(staticStandardSpriteSize, 38);
    [HideInInspector] private static Vector2 staticRotationCableSize = new Vector2(staticCableWidith, staticCableWidith);
    [HideInInspector] private static Vector2 staticStraightCableSize = new Vector2(staticCableWidith, staticJointDistance);

    [HideInInspector] private static float staticTableSnapDistance = staticJointDistance/2;
    [HideInInspector] private static float   staticTableTopDistanceFromLeg = 18;
    [HideInInspector] private static Vector2 staticStartingPlugOffset = new Vector2(3, 3);
    [HideInInspector] private static float staticStartingPlugOffsetRightSideAdd = 1f;

    
    //Game distances and offsets
    [HideInInspector] public float   plugLockingDistance;
    [HideInInspector] public float   standardSpriteSize;
    [HideInInspector] public float   cableWidith;
    [HideInInspector] public float   rotationAndStraightCableOverlap;

    [HideInInspector] public float   electricalStripSeparatorSize;
    [HideInInspector] public Vector2 electricalStripBaseSize;
    [HideInInspector] public float   jointDistance;
    [HideInInspector] public Vector2 powerSwitchBaseSize;
    [HideInInspector] public Vector2 rotationCableSize;
    [HideInInspector] public Vector2 straightCableSize;

    [HideInInspector] public float   tableSnapDistance;
    [HideInInspector] public float   tableTopDistanceFromLeg;
    [HideInInspector] public Vector2 startingPlugOffset;
    [HideInInspector] public float   startingPlugOffsetRightSideAdd;

    void Awake() {
        instance = this;
        RenewValues();
    }
    
 
    void Update() {
    }


    private void RenewLevelScale() {
        Vector2 normalSize = new Vector2(1920, 1980);
        Vector2 sizeRatio = new Vector2(normalSize.x/Screen.width, normalSize.y/Screen.height);
        float scale = 1/sizeRatio.x;
        if(sizeRatio.x > sizeRatio.y) {
            scale = 1/sizeRatio.y;
        }
        levelScale = scale*scaleMultiplier;
    }

    void RenewValues() {
        RenewLevelScale();

        plugLockingDistance             = staticPlugLockingDistance*levelScale*scaleMultiplier;
        standardSpriteSize              = staticStandardSpriteSize*levelScale*scaleMultiplier;
        cableWidith                     = staticCableWidith*levelScale*scaleMultiplier;
        rotationAndStraightCableOverlap = staticRotationAndStraightCableOverlap*levelScale*scaleMultiplier;
        electricalStripSeparatorSize    = staticElectricalStripSeparatorSize*levelScale*scaleMultiplier;
        electricalStripBaseSize         = staticElectricalStripBaseSize*levelScale*scaleMultiplier;
        jointDistance                   = staticJointDistance*levelScale*scaleMultiplier;
        powerSwitchBaseSize             = staticPowerSwitchBaseSize*levelScale*scaleMultiplier;
        rotationCableSize               = staticRotationCableSize*levelScale*scaleMultiplier;
        straightCableSize               = staticStraightCableSize*levelScale*scaleMultiplier;
        tableSnapDistance               = staticTableSnapDistance*levelScale*scaleMultiplier;
        tableTopDistanceFromLeg         = staticTableTopDistanceFromLeg*levelScale*scaleMultiplier;
        startingPlugOffset              = staticStartingPlugOffset*levelScale*scaleMultiplier;
        startingPlugOffsetRightSideAdd  = staticStartingPlugOffsetRightSideAdd*levelScale*scaleMultiplier;
    }
}
