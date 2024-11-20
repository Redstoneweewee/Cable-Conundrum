using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdminToggles : MonoBehaviour {
    [SerializeField] public bool editorMode;
    public bool cachedEditorMode;

    void Awake() {
        cachedEditorMode = !editorMode;
    }
}
