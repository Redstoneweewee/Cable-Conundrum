
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ButtonsGlobal : MonoBehaviour {

    public void DisableButton(Button button) {
        button.enabled = false;
        Utilities.TryGetComponent<Image>(button.gameObject).enabled = false;
        Utilities.TryGetComponentInChildren<TextMeshProUGUI>(button.gameObject).enabled = false;
    }
    public void EnableButton(Button button) {
        button.enabled = true;
        Utilities.TryGetComponent<Image>(button.gameObject).enabled = true;
        Utilities.TryGetComponentInChildren<TextMeshProUGUI>(button.gameObject).enabled = true;
    }

    public void SubscribeToButton(Button button, System.Action function) {
        button.onClick.AddListener(() => function.Invoke());
    }

    public void SetButtonText(Button button, string text) {
        TextMeshProUGUI textBox = Utilities.TryGetComponentInChildren<TextMeshProUGUI>(button.gameObject);
        textBox.text = text;
    }
}
