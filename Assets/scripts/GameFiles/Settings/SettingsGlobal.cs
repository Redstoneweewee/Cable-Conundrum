using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGlobal : MonoBehaviour {
    [SerializeField] private GameObject settingsCanvas;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    

    public void OnPressEnterSettingsButton() {
        settingsCanvas.SetActive(true);
    }

    public void OnPressExitSettingsButton() {
        settingsCanvas.SetActive(false);
    }
}
