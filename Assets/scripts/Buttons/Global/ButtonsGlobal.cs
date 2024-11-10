
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ButtonsGlobal : MonoBehaviour {
    [HideInInspector] public ScenesController scenesController;
    [HideInInspector] public SettingsController settingsController;
    [SerializeField]  public List<ButtonAtributes> buttonAttributes = new List<ButtonAtributes>();

    void Awake() {
        scenesController = FindObjectOfType<ScenesController>();
    }

    void Start() {
        foreach(ButtonAtributes buttonAttribute in buttonAttributes) {
            buttonAttribute.Button = Utilities.TryGetComponentInChildren<Button>(buttonAttribute.ButtonGameObject);

            switch(buttonAttribute.ButtonType) {
                case ButtonTypes.EnterLevel:
                    SubscribeToButton(buttonAttribute.Button, scenesController.OnPressEnterLevelButton);
                    break;
                case ButtonTypes.EnterMenu:
                    SubscribeToButton(buttonAttribute.Button, scenesController.OnPressEnterMenuButton);
                    break;
                case ButtonTypes.EnterSettings:
                    SubscribeToButton(buttonAttribute.Button, settingsController.OnPressEnterSettingsButton);
                    break;
                case ButtonTypes.ExitSettings:
                    SubscribeToButton(buttonAttribute.Button, settingsController.OnPressExitSettingsButton);
                    break;
            }
        }
    }

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
