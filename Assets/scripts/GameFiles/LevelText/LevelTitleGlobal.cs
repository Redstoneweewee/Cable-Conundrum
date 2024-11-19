using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTitleGlobal : MonoBehaviour {
    LevelInitializerGlobal levelInitializerGlobal;
    void Awake() {
        levelInitializerGlobal = FindObjectOfType<LevelInitializerGlobal>();
        TextMeshProUGUI text = Utilities.TryGetComponent<TextMeshProUGUI>(gameObject);
        text.text = "LEVEL "+(levelInitializerGlobal.levelIndex + Constants.firstLevelBuidIndex - 1);
    }

    // Update is called once per frame
    void Update() {

    }
}
