using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGlobal : MonoBehaviour, IDataPersistence {
    [SerializeField] private GameObject settingsCanvas;
    private SoundsData soundsData;

    void Awake() {
        soundsData = FindFirstObjectByType<SoundsData>();
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
        Utilities.TryGetComponent<Canvas>(settingsCanvas).sortingOrder = Constants.settingsCanvasSortOrder;
    }

    public void OnPressExitSettingsButton() {
        Utilities.TryGetComponent<Canvas>(settingsCanvas).sortingOrder = Constants.deactivatedCanvasSortOrder;
    }
}
