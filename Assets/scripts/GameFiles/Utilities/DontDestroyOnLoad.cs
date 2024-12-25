using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : Singleton<DontDestroyOnLoad> {

    public override void OnAwake() {
        DontDestroyOnLoad[] dontDestroyOnLoad = FindObjectsByType<DontDestroyOnLoad>(FindObjectsSortMode.None);
        if(dontDestroyOnLoad.Length > 1) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
