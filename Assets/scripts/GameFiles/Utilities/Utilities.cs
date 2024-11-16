using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Utilities : MonoBehaviour {

    public static Object TryGetComponent<Object>(GameObject gameObject) {
        Object objectTypeOut;
        if(gameObject.TryGetComponent<Object>(out objectTypeOut)) { return objectTypeOut; }
        else {
            Debug.LogWarning($"Utilities.TryGetComponent on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
    }
    public static Object[] TryGetComponents<Object>(GameObject gameObject) {
        Object[] objectTypeOut = gameObject.GetComponents<Object>();
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponents on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object TryGetComponentInChildren<Object>(GameObject gameObject) {
        Object objectTypeOut = gameObject.GetComponentInChildren<Object>();
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponentInChildren on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object[] TryGetComponentsInChildren<Object>(GameObject gameObject) {
        Object[] objectTypeOut = gameObject.GetComponentsInChildren<Object>();
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponentsInChildren on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object TryGetComponentInParent<Object>(GameObject gameObject) {
        Object objectTypeOut = gameObject.GetComponentInParent<Object>();
        if(objectTypeOut == null) {
            Debug.LogWarning($"Utilities.TryGetComponentInParent on {gameObject.name} failed to get for component {typeof(Object)}.");
            return default;
        }
        else { return objectTypeOut; }
    }
    public static Object[] TryGetComponentsInParent<Object>(GameObject gameObject) {
        Object[] objectTypeOut = gameObject.GetComponentsInParent<Object>();
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


    public static void InheritAllPlugAttributes(GameObject receiver, SavePlug provider) {
        Debug.Log($"Inheriting values - receiver: {receiver.name}");
        Debug.Log($"plugPosition changed: {receiver.name} from ({receiver.transform.position}) to ({provider.plugPosition})");
        receiver.transform.position = provider.plugPosition;
        Debug.Log($"isPluggedIn changed: {receiver.name} from ({TryGetComponent<PlugAttributes>(receiver).isPluggedIn}) to ({provider.isPluggedIn})");
        TryGetComponent<PlugAttributes>(receiver).isPluggedIn = provider.isPluggedIn;
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
