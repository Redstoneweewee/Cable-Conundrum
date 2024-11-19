using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGlobal : MonoBehaviour {
    [SerializeField] private GameObject settingsCanvas;

    

    public void OnPressEnterSettingsButton() {
        settingsCanvas.SetActive(true);
    }

    public void OnPressExitSettingsButton() {
        settingsCanvas.SetActive(false);
    }
}
