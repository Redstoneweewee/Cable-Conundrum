using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class Utilities {

    public static Object TryGetComponent<Object>(GameObject gameObject) {
        if(gameObject == null) {
            Debug.LogWarning($"Utilities.TryGetComponent failed to get for component {typeof(Object)} because gameObject was null.");
            return default;
        }
        Object objectTypeOut;
        if(gameObject.TryGetComponent<Object>(out objectTypeOut)) { return objectTypeOut; }
        else {
            Debug.LogWarning($"Utilities.TryGetComponent on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
    }
    public static Object[] TryGetComponents<Object>(GameObject gameObject) {
        if(gameObject == null) {
            Debug.LogWarning($"Utilities.TryGetComponents failed to get for component {typeof(Object)} because gameObject was null.");
            return default;
        }
        Object[] objectTypeOut = gameObject.GetComponents<Object>();
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponents on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object TryGetComponentInChildren<Object>(GameObject gameObject, bool includingInactive = false) {
        if(gameObject == null) {
            Debug.LogWarning($"Utilities.TryGetComponentInChildren failed to get for component {typeof(Object)} because gameObject was null.");
            return default;
        }
        Object objectTypeOut = gameObject.GetComponentInChildren<Object>(includingInactive);
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponentInChildren on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object[] TryGetComponentsInChildren<Object>(GameObject gameObject, bool includingInactive = false) {
        if(gameObject == null) {
            Debug.LogWarning($"Utilities.TryGetComponentsInChildren failed to get for component {typeof(Object)} because gameObject was null.");
            return default;
        }
        Object[] objectTypeOut = gameObject.GetComponentsInChildren<Object>(includingInactive);
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponentsInChildren on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object TryGetComponentInParent<Object>(GameObject gameObject, bool includingInactive = false) {
        if(gameObject == null) {
            Debug.LogWarning($"Utilities.TryGetComponentInParent failed to get for component {typeof(Object)} because gameObject was null.");
            return default;
        }
        Object objectTypeOut = gameObject.GetComponentInParent<Object>(includingInactive);
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponentInParent on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object[] TryGetComponentsInParent<Object>(GameObject gameObject, bool includingInactive = false) {
        if(gameObject == null) {
            Debug.LogWarning($"Utilities.TryGetComponentsInParent failed to get for component {typeof(Object)} because gameObject was null.");
            return default;
        }
        Object[] objectTypeOut = gameObject.GetComponentsInParent<Object>(includingInactive);
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponentsInParent on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }


    public static void DisableButton(Button button) {
        button.enabled = false;
        TryGetComponent<Image>(button.gameObject).enabled = false;
        TryGetComponentInChildren<TextMeshProUGUI>(button.gameObject).enabled = false;
    }
    public static void EnableButton(Button button) {
        button.enabled = true;
        TryGetComponent<Image>(button.gameObject).enabled = true;
        TryGetComponentInChildren<TextMeshProUGUI>(button.gameObject).enabled = true;
    }
    public static void SetButtonText(Button button, string text) {
        TextMeshProUGUI textBox = TryGetComponentInChildren<TextMeshProUGUI>(button.gameObject);
        textBox.text = text;
    }

    public static void SubscribeToButton(Button button, System.Action function) {
        button.onClick.AddListener(() => function.Invoke());
    }

    public static void SubscribeToButton<T>(Button button, UnityAction<T> function, T argument) {
        button.onClick.AddListener(() => function.Invoke(argument));
    }

    public static void SubscribeToSlider(Slider slider, UnityAction<float> function) {
        slider.onValueChanged.AddListener((value) => function.Invoke(value));
    }


    public static void SetText(GameObject gameObject, string text) {
        TextMeshProUGUI textBox = TryGetComponent<TextMeshProUGUI>(gameObject);
        textBox.text = text;
    }
    public static string GetText(GameObject gameObject) {
        TextMeshProUGUI textBox = TryGetComponent<TextMeshProUGUI>(gameObject);
        return textBox.text;
    }



    public static bool IsApproximate(Quaternion q1, Quaternion q2, float precision) {
        return Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;
    }
    public static bool IsApproximate(Vector3 v1, Vector3 v2, float precision) {
        return (v2-v1).magnitude <= precision;
    }
    public static bool IsApproximate(float f1, float f2, float precision) {
        return Math.Abs(f1-f2) <= precision;
    }
    


    public static Index2D ClampIndex2D(Index2D index2D, int lowestX, int highestX, int lowestY, int highestY) {
        if(index2D.x < lowestX) { index2D.x = lowestX; }
        if(index2D.x > highestX) { index2D.x = highestX; }
        if(index2D.y < lowestY) { index2D.y = lowestY; }
        if(index2D.y > highestY) { index2D.y = highestY; }
        return index2D;
    }

    public static Index2D CalculateJointsGridIndex(Vector3 position) {
        Transform[,] jointsGrid = GridsData.Instance.jointsGrid;
        float   subJointLength  = LevelResizeGlobal.Instance.jointDistance/2;

        Vector2 distanceFromTopLeftJoint = new Vector2(position.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - position.y);
        Index2D jointsGridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
        jointsGridIndex          = new Index2D(Math.Clamp(jointsGridIndex.y, 0, jointsGrid.GetLength(0)-1), Math.Clamp(jointsGridIndex.x, 0, jointsGrid.GetLength(1)-1));
        return jointsGridIndex;
    }
    
    public static Index2D CalculateSocketsGridIndex(Vector3 position) {
        Transform[,] socketsGrid = GridsData.Instance.socketsGrid;
        float   subSocketLength  = LevelResizeGlobal.Instance.jointDistance;
        Vector2 distanceFromTopLeftSocket = new Vector2(position.x - socketsGrid[0,0].position.x, socketsGrid[0,0].position.y - position.y);
        Index2D socketsGridIndex = new Index2D(((int)(distanceFromTopLeftSocket.x/subSocketLength)+1)/2, ((int)(distanceFromTopLeftSocket.y/subSocketLength)+1)/2);
        socketsGridIndex         = new Index2D(Math.Clamp(socketsGridIndex.y, 0, socketsGrid.GetLength(0)-1), Math.Clamp(socketsGridIndex.x, 0, socketsGrid.GetLength(1)-1));
        return socketsGridIndex;
    }

    public static void SetCablesOpacity(GameObject cableParent, float opacity) {
        CanvasGroup canvasGroup = TryGetComponent<CanvasGroup>(cableParent);
        canvasGroup.alpha = opacity;
    }
    public static void ModifyCableColorsToObstacle(GameObject cable) {
        Image cableImage = TryGetComponentInChildren<Image>(cable);
        cableImage.color = new Color(Constants.obstacleCableColor.r,
                                        Constants.obstacleCableColor.g,
                                        Constants.obstacleCableColor.b,
                                        cableImage.color.a);
    }

    public static void ModifyCableValues(Transform currentCable, CableChildAttributes newAttributes, bool isRotationCable,
                                   float newZRotation, Vector2 newSize, Vector2 newPivot, Sprite newSprite) {
        
        TryGetComponentInChildren<Image>(currentCable.gameObject).overrideSprite = newSprite;
        currentCable.rotation = Quaternion.Euler(0, 0, newZRotation);
        TryGetComponentInChildren<RectTransform>(currentCable.gameObject).sizeDelta = newSize;
        
        RectTransform cableRectTransform = TryGetComponentInChildren<RectTransform>(currentCable.gameObject);
        cableRectTransform.pivot = newPivot;

        CableChildAttributes currentAttributes = TryGetComponent<CableChildAttributes>(currentCable.gameObject);
        InheritCableAttributes(currentAttributes, newAttributes, isRotationCable);
    }

    public static void InheritCableAttributes(CableChildAttributes receiver, CableChildAttributes provider, bool isRotationCable) {
        receiver.isRotationCable   = isRotationCable;
        receiver.cableType         = provider.cableType;
        receiver.cableSpriteIndex  = provider.cableSpriteIndex;
        receiver.zRotation         = provider.zRotation;
        receiver.pivot             = provider.pivot;
        receiver.shadowDirection   = provider.shadowDirection;
        receiver.startingDirection = provider.startingDirection;
        receiver.endingDirection   = provider.endingDirection;
        receiver.directionMultiple = provider.directionMultiple;
    }

    
    

    public static ShadowDirections GetShadowDirectionForStraightCables(ShadowDirections previousShadowDirection, Directions startingDirection, bool isRotationCable) {
        if(!isRotationCable) {
            return previousShadowDirection;
        }
        else {
            if(previousShadowDirection == ShadowDirections.In) {
                if     (startingDirection == Directions.Up   ) { return ShadowDirections.Down;  }
                else if(startingDirection == Directions.Down ) { return ShadowDirections.Up;    }
                else if(startingDirection == Directions.Left ) { return ShadowDirections.Right; }
                else if(startingDirection == Directions.Right) { return ShadowDirections.Left;  }
            }
            else {
                return (ShadowDirections)startingDirection;
            }
        }
        Debug.LogError("GetShadowDirectionForStraightCables function did not work correctly. None of the conditions were met.");
        return ShadowDirections.Down; //should never get here
    }

    //does not check for rotation cables (because 2 rotation cables will never be adjacent)
    public static ShadowDirections GetShadowDirectionForRotationCables(ShadowDirections previousShadowDirection, Directions endingDirection) {
        if(previousShadowDirection == ShadowDirections.Up) {
            if     (endingDirection == Directions.Up  ) { return ShadowDirections.In; }
            else if(endingDirection == Directions.Down) { return ShadowDirections.Out; }
        }
        else if(previousShadowDirection == ShadowDirections.Down) {
            if     (endingDirection == Directions.Up  ) { return ShadowDirections.Out; }
            else if(endingDirection == Directions.Down) { return ShadowDirections.In; }
        }
        else if(previousShadowDirection == ShadowDirections.Left) {
            if     (endingDirection == Directions.Left ) { return ShadowDirections.In; }
            else if(endingDirection == Directions.Right) { return ShadowDirections.Out; }
        }
        else if(previousShadowDirection == ShadowDirections.Right) {
            if     (endingDirection == Directions.Left ) { return ShadowDirections.Out; }
            else if(endingDirection == Directions.Right) { return ShadowDirections.In; }
        }
        Debug.LogError("GetShadowDirectionForRotationCables function did not work correctly. None of the conditions were met."+
                       "\npreviousShadowDirection: "+previousShadowDirection+
                       "\nendingDirection: "+endingDirection);
        return ShadowDirections.In; //should never get here
    }

    public static GameObject GetStraightCablePrefab(CablePrefabs cablePrefabs, ShadowDirections shadowDirection, Directions startDirection) {
        if(shadowDirection == ShadowDirections.Up) {
            if(startDirection == Directions.Left)       { return cablePrefabs.cablePrefabs[0]; }
            else if(startDirection == Directions.Right) { return cablePrefabs.cablePrefabs[1]; }
        }
        else if(shadowDirection == ShadowDirections.Down) {
            if(startDirection == Directions.Left)       { return cablePrefabs.cablePrefabs[2]; }
            else if(startDirection == Directions.Right) { return cablePrefabs.cablePrefabs[3]; }
        }
        else if(shadowDirection == ShadowDirections.Left) {
            if(startDirection == Directions.Up)        { return cablePrefabs.cablePrefabs[4]; }
            else if(startDirection == Directions.Down) { return cablePrefabs.cablePrefabs[5]; }
        }
        else if(shadowDirection == ShadowDirections.Right) {
            if(startDirection == Directions.Up)        { return cablePrefabs.cablePrefabs[6]; }
            else if(startDirection == Directions.Down) { return cablePrefabs.cablePrefabs[7]; }
        }
        Debug.LogError("GetStraightCablePrefab function did not work correctly. None of the conditions were met.");
        return cablePrefabs.cablePrefabs[0]; //should never get here
    }
    
    public static GameObject GetRotationCablePrefab(CablePrefabs cablePrefabs, ShadowDirections shadowDirection, Directions startDirection, Directions endDirection) {
        if(shadowDirection == ShadowDirections.In) {
            if(startDirection == Directions.Up) {
                if     (endDirection == Directions.Left)  { return cablePrefabs.cablePrefabs[8]; }
                else if(endDirection == Directions.Right) { return cablePrefabs.cablePrefabs[9]; }
            }
            else if(startDirection == Directions.Down) { 
                if     (endDirection == Directions.Left)  { return cablePrefabs.cablePrefabs[10]; }
                else if(endDirection == Directions.Right) { return cablePrefabs.cablePrefabs[11]; }
            }
            else if(startDirection == Directions.Left) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs.cablePrefabs[12]; }
                else if(endDirection == Directions.Down) { return cablePrefabs.cablePrefabs[13]; }
            }
            else if(startDirection == Directions.Right) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs.cablePrefabs[14]; }
                else if(endDirection == Directions.Down) { return cablePrefabs.cablePrefabs[15]; }
            }
        }
        else if(shadowDirection == ShadowDirections.Out) {
            if(startDirection == Directions.Up) {
                if     (endDirection == Directions.Left)  { return cablePrefabs.cablePrefabs[16]; }
                else if(endDirection == Directions.Right) { return cablePrefabs.cablePrefabs[17]; }
            }
            else if(startDirection == Directions.Down) { 
                if     (endDirection == Directions.Left)  { return cablePrefabs.cablePrefabs[18]; }
                else if(endDirection == Directions.Right) { return cablePrefabs.cablePrefabs[19]; }
            }
            else if(startDirection == Directions.Left) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs.cablePrefabs[20]; }
                else if(endDirection == Directions.Down) { return cablePrefabs.cablePrefabs[21]; }
            }
            else if(startDirection == Directions.Right) { 
                if     (endDirection == Directions.Up)  { return cablePrefabs.cablePrefabs[22]; }
                else if(endDirection == Directions.Down) { return cablePrefabs.cablePrefabs[23]; }
            }
        }
        Debug.LogError("GetRotationCablePrefab function did not work correctly. None of the conditions were met."+
                       "\nstartDirection: "+startDirection+
                       "\nendingDirection: "+endDirection);
        return cablePrefabs.cablePrefabs[8]; //should never get here
    }



    public static float Orientation(Vector2 A, Vector2 B, Vector2 C) {
        return (B.y-A.y) * (C.x-B.x) - (B.x-A.x) * (C.y-B.y);
    }

    /// <summary>
    /// Warning: Does not calculate overlapping lines! Overlapping lines will return false.
    /// </summary>
    public static bool Intercepts(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2) {
        float o1 = Orientation(p1, p2, q1);
        float o2 = Orientation(p1, p2, q2);
        float o3 = Orientation(q1, q2, p1);
        float o4 = Orientation(q1, q2, p2);

        if(((o1 > 0 && o2 < 0) || (o1 < 0 && o2 > 0)) && ((o3 > 0 && o4 < 0) || (o3 < 0 && o4 > 0))) {
            return true;
        }
        return false;
    }

    
    public static Vector2[] CreateCorners(Vector2 p1, Vector2 p2) {
        // Min and Max are used to get the 2 corners, regardless of drag direction.
        var bottomLeft = Vector3.Min(p1, p2);
        var topRight = Vector3.Max(p1, p2);

        Vector2[] corners = new Vector2[]
        {
            new Vector2(bottomLeft.x, topRight.y), // Top-left
            new Vector2(topRight.x, topRight.y), // Top-right
            new Vector2(bottomLeft.x, bottomLeft.y), // Bottom-left
            new Vector2(topRight.x, bottomLeft.y) // Bottom-right
        };
        return corners;
    }


    public static Vector3 GetRayHitPosition(Ray ray, LayerMask layerMask, float maxDistance) {
        bool raycast = Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask);

        // If nothing was hit, use the max distance point
        return raycast ? hit.point : ray.GetPoint(maxDistance);
    }

}
