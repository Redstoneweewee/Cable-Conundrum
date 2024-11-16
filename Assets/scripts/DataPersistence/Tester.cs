using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour, IDataPersistence {
    private int testingInt = 0;

    public void LoadData(GameData data) {
        this.testingInt = data.testingInt;
    }
    public void SaveData(ref GameData data) {
        data.testingInt = this.testingInt;
    }

    public void Start() {
        UpdateVisual();
    }

    public void OnPressButton() {
        testingInt++;
        UpdateVisual();
    }

    private void UpdateVisual() {
        Utilities.SetText(gameObject, testingInt.ToString());
    }
}
