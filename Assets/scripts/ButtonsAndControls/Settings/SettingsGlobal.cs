using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGlobal : DataPersistentSingleton<SettingsGlobal> {
    [SerializeField] private GameObject settingsCanvas;

    public override IEnumerator Initialize() {
        yield return null;
    }

    public override void LoadData(GameData data) {
        SoundsData.Instance.soundVolume = data.settings.soundVolume;
        SoundsData.Instance.musicVolume = data.settings.musicVolume;
        SoundsController.Instance.InitializeSliders();
    }

    public override void SaveData(GameData data) {
        data.settings.soundVolume = SoundsData.Instance.soundVolume;
        data.settings.musicVolume = SoundsData.Instance.musicVolume;
    }


    public void OnPressEnterSettingsButton() {
        Utilities.TryGetComponent<Canvas>(settingsCanvas).sortingOrder = Constants.settingsCanvasSortOrder;
    }

    public void OnPressExitSettingsButton() {
        Utilities.TryGetComponent<Canvas>(settingsCanvas).sortingOrder = Constants.deactivatedCanvasSortOrder;
    }
}
