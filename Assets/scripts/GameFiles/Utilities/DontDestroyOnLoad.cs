using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : ScriptInitializerBase {

    public override IEnumerator Initialize() {
        DontDestroyOnLoad[] dontDestroyOnLoad = FindObjectsByType<DontDestroyOnLoad>(FindObjectsSortMode.None);
        if(dontDestroyOnLoad.Length > 1) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        yield return null;
    }
}
