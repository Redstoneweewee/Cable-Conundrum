using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour, IDataPersistence {
    private int testingInt = 0;

    public IEnumerator LoadData(GameData data) {
        yield return null;
        this.testingInt = data.testingInt;
        UpdateVisual();
    }
    public void SaveData(GameData data) {
        data.testingInt = this.testingInt;
    }

    public void OnPressButton() {
        testingInt++;
        UpdateVisual();
    }

    private void UpdateVisual() {
        Utilities.SetText(gameObject, testingInt.ToString());
    }
}
