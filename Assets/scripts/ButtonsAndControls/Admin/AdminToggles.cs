using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdminToggles : Singleton<AdminToggles> {
    [SerializeField] public bool editorMode;
    public bool cachedEditorMode;

    public override IEnumerator Initialize() {
        cachedEditorMode = !editorMode;
        yield return null;
    }
}
