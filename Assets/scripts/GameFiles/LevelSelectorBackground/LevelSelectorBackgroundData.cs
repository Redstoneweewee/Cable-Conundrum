using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelSelectorBackgroundData : Singleton<LevelSelectorBackgroundData> {

    [SerializeField]  public GameObject backgroundImageOnly;
    [SerializeField]  public GameObject buttonsImageOnly;
    [HideInInspector] public Mouse mouse;
    public Vector2 center;
    [HideInInspector] public Vector2 deltaMousePosition;
    
    [SerializeField] [Range(0.01f, 1)] public float mouseMoveInterpolation = 0.1f;
    [SerializeField] [Range(0.01f, 1)] public float backgroundMovementMultiplier = 0.02f;
    [SerializeField] [Range(0.01f, 1)] public float buttonsMovementMultiplier = 0.01f;
    [HideInInspector] public Vector2 targetBackgroundImagePosition;
    [HideInInspector] public Vector2 targetButtonsImagePosition;



    public override void OnAwake() {
        mouse = Mouse.current;
        center = transform.position;
    }
}
