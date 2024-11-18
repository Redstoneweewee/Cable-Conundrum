using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    void Awake() {
        DontDestroyOnLoad[] dontDestroyOnLoad = FindObjectsOfType<DontDestroyOnLoad>();
        if(dontDestroyOnLoad.Length > 1) {
            Debug.Log("destroyed "+gameObject.name);
            Destroy(gameObject);
        }
        Debug.Log("kept "+gameObject.name);
        DontDestroyOnLoad(gameObject);
    }
}
