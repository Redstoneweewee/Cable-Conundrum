using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTitleGlobal : Singleton<LevelTitleGlobal> {
    public override IEnumerator Initialize() {
        yield return null;
    }
    public override void OnAwake() { }

    void Start() {
        TextMeshProUGUI text = Utilities.TryGetComponent<TextMeshProUGUI>(gameObject);
        text.text = "LEVEL "+(LevelInitializerGlobal.Instance.levelIndex + 1);
    }
}
