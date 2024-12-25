using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGlobal : Singleton<SettingsGlobal>, IDataPersistence {
    [SerializeField] private GameObject settingsCanvas;

    public override void OnAwake() {}

    public IEnumerator LoadData(GameData data) {
        yield return null;
        SoundsData.Instance.soundVolume = data.settings.soundVolume;
        SoundsData.Instance.musicVolume = data.settings.musicVolume;
    }

    public void SaveData(GameData data) {
        data.settings.soundVolume = SoundsData.Instance.soundVolume;
        data.settings.musicVolume = SoundsData.Instance.musicVolume;
    }
    public void SaveDataLate(GameData data) {}


    public void OnPressEnterSettingsButton() {
        Utilities.TryGetComponent<Canvas>(settingsCanvas).sortingOrder = Constants.settingsCanvasSortOrder;
    }

    public void OnPressExitSettingsButton() {
        Utilities.TryGetComponent<Canvas>(settingsCanvas).sortingOrder = Constants.deactivatedCanvasSortOrder;
    }
}
