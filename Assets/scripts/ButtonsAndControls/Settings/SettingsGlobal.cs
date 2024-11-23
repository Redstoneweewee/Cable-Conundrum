using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGlobal : MonoBehaviour, IDataPersistence {
    [SerializeField] private GameObject settingsCanvas;
    private SoundsData soundsData;

    void Awake() {
        soundsData = FindObjectOfType<SoundsData>();
    }

    public IEnumerator LoadData(GameData data) {
        yield return null;
        soundsData.soundVolume = data.settings.soundVolume;
        soundsData.musicVolume = data.settings.musicVolume;
    }

    public void SaveData(GameData data) {
        data.settings.soundVolume = soundsData.soundVolume;
        data.settings.musicVolume = soundsData.musicVolume;
    }
    public void SaveDataLate(GameData data) {}


    public void OnPressEnterSettingsButton() {
        settingsCanvas.SetActive(true);
    }

    public void OnPressExitSettingsButton() {
        settingsCanvas.SetActive(false);
    }
}
