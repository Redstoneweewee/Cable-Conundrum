using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitializerGlobal : InitializerBase {
    new void Awake() {
        base.Awake();
        finishedWithAllTasks = true;
        StartCoroutine(base.SetMenuButton(false));
        StartCoroutine(base.SetLevelSelectorButton(false));
        StartCoroutine(base.SetTutorialHelpButton(true));
    }
}
