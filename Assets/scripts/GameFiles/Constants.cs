using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum InteractableType { Plug, Cable, PowerSwitch };

public class Constants {
    public static int firstLevelBuidIndex = 2;
    public static int numberOfLevels = 10;

    public static int deactivatedCanvasSortOrder = -1;
    public static int settingsCanvasSortOrder = 50;
    public static int tutorialCanvasSortOrder = 60;
    public static int exitConfirmationCanvasSortOrder = 70;

    public static Color levelSelectorFinishedButtonColor1 = RemapColorFromRGB(new Color(134, 172, 114, 255));
    public static Color levelSelectorFinishedButtonColor2 = RemapColorFromRGB(new Color(137, 186, 111, 255));
    public static Color levelSelectorFinishedButtonColor3 = RemapColorFromRGB(new Color(109, 152, 86, 255));



    [Range(0.01f, 1)] public static float plugInterpolation = 0.2f;
    public static int plugLockingDistance = 28;
    public static int standardSpriteSize = 140;
    public static int cableWidith = 26;
    public static float rotationAndStraightCableOverlap = 13;

    public static int electricalStripSeparatorSize = 30;
    public static Vector2 electricalStripBaseSize = new Vector2(standardSpriteSize, standardSpriteSize);
    public static float   jointDistance       = (electricalStripBaseSize.x + electricalStripSeparatorSize)/2; // = 85
    public static Vector2 powerSwitchBaseSize = new Vector2(standardSpriteSize, 38);
    public static Vector2 rotationCableSize = new Vector2(cableWidith, cableWidith);
    public static Vector2 straightCableSize = new Vector2(cableWidith, jointDistance);

    public static float cableOpacity = 0.8f;

    public static Color cableIntersectionColor = RemapColorFromRGB(new Color(255, 134, 134, 255));
    public static float joinOpacityMin = RemapOpacityFromRGB(0);
    public static float joinOpacityMid = RemapOpacityFromRGB(180);
    public static float joinOpacityMax = RemapOpacityFromRGB(220);
    public static Color jointColor = RemapColorFromRGB(new Color(160, 95, 155, 0));


    public static Color obstacleCableColor = RemapColorFromRGB(new Color(193, 193, 193, 255));
    public static Color obstacleCableIntersectionColor = RemapColorFromRGB(new Color(193, 100, 100, 255));
    public static float tableSnapDistance = jointDistance/2;


    public static Vector2 startingPlugOffset = new Vector2(3, 3);
    public static float startingPlugOffsetRightSideAdd = 1f;

    
    public static Color RemapColorFromRGB(Color color) {
        float4 newColorValues = math.remap(new float4(0.0f, 0.0f, 0.0f, 0.0f), 
                                           new float4(255.0f, 255.0f, 255.0f, 255.0f), 
                                           new float4(0.0f, 0.0f, 0.0f, 0.0f), 
                                           new float4(1.0f, 1.0f, 1.0f, 1.0f), 
                                           new float4(color.r, color.g, color.b, color.a));
        return new Color(newColorValues.x, newColorValues.y, newColorValues.z, newColorValues.w);
    }
    public static float RemapOpacityFromRGB(float opacity) {
        float newColorValues = math.remap(0f, 255f, 0f, 1f, opacity);
        return newColorValues;
    }
}
