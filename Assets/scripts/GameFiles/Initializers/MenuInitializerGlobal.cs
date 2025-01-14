using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitializerGlobal : InitializerBase<MenuInitializerGlobal> {
    public override IEnumerator Initialize() {
        StartCoroutine(base.Initialize());
        //finishedWithAllTasks = true;
        StartCoroutine(base.SetMenuButton(false));
        StartCoroutine(base.SetLevelSelectorButton(false));
        StartCoroutine(base.SetTutorialHelpButton(false));
        yield return null;
    }

    public override void LoadData(GameData data) { }

    public override void SaveData(GameData data) { }

}
