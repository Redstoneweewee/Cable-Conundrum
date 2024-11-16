using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitializerGlobal : InitializerBase {
    void Awake() {
        finishedWithAllTasks = true;
        StartCoroutine(base.SetMenuButton(false));
        StartCoroutine(base.SetLevelSelectorButton(false));
    }
}
