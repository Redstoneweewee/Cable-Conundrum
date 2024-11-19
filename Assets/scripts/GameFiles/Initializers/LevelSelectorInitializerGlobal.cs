using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorInitializerGlobal : InitializerBase {
    new void Awake() {
        base.Awake();
        finishedWithAllTasks = true;
        StartCoroutine(base.SetMenuButton(true));
        StartCoroutine(base.SetLevelSelectorButton(false));
    }
}
