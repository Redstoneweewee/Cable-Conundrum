using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTitleGlobal : Singleton<LevelTitleGlobal> {
    public override IEnumerator Initialize() {
        TextMeshProUGUI text = Utilities.TryGetComponent<TextMeshProUGUI>(gameObject);
        text.text = "LEVEL "+(LevelInitializerGlobal.Instance.levelIndex + 1);
        yield return null;
    }
}
