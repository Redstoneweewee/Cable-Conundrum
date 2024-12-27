using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitializerGlobal : InitializerBase<MenuInitializerGlobal> {
    public override void OnAwake() {
        finishedWithAllTasks = true;
        StartCoroutine(base.SetMenuButton(false));
        StartCoroutine(base.SetLevelSelectorButton(false));
        StartCoroutine(base.SetTutorialHelpButton(false));
    }
}
