using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasEnums : MonoBehaviour {
    [SerializeField] private CanvasType canvasType;
    public CanvasType CanvasType {get{return canvasType;} private set{}}
}

[Serializable]
public enum CanvasType {
    None,
    GameCanvas,
    PlugSelectorCanvas
}