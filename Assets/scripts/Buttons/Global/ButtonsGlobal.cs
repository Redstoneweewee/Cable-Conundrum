
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ButtonsGlobal : MonoBehaviour {
    [HideInInspector] public ScenesController scenesController;
    [SerializeField]  public List<ButtonAtributes> buttonAttributes = new List<ButtonAtributes>();

    void Awake() {
        scenesController = FindObjectOfType<ScenesController>();
    }

    void Start() {
        foreach(ButtonAtributes buttonAttribute in buttonAttributes) {
            buttonAttribute.Button = buttonAttribute.ButtonGameObject.GetComponentInChildren<Button>();

            switch(buttonAttribute.ButtonType) {
                case ButtonTypes.StartGame:
                    SubscribeToButton(buttonAttribute.Button, scenesController.OnPressStartButton);
                    break;
                case ButtonTypes.ExitToMenu:
                    SubscribeToButton(buttonAttribute.Button, scenesController.OnPressExitToMenuButton);
                    break;
            }
        }
    }

    public void DisableButton(Button button) {
        button.enabled = false;
        button.GetComponent<Image>().enabled = false;
        button.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }
    public void EnableButton(Button button) {
        button.enabled = true;
        button.GetComponent<Image>().enabled = true;
        button.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    public void SubscribeToButton(Button button, System.Action function) {
        button.onClick.AddListener(() => function.Invoke());
    }

    public void SetButtonText(Button button, string text) {
        TextMeshProUGUI textBox = button.GetComponentInChildren<TextMeshProUGUI>();
        textBox.text = text;
    }
}
