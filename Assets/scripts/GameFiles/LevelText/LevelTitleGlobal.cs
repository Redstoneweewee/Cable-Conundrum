using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTitleGlobal : Singleton<LevelTitleGlobal> {
    public override void OnAwake() { }

    void Update() {
        TextMeshProUGUI text = Utilities.TryGetComponent<TextMeshProUGUI>(gameObject);
        text.text = "LEVEL "+(LevelInitializerGlobal.Instance.levelIndex + Constants.firstLevelBuidIndex - 1);
    }
}
