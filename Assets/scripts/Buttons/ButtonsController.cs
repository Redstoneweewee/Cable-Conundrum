
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonTypes { StartGame, ExitToMenu }

[Serializable]
public class ButtonAtributes {
    [SerializeField] private GameObject buttonGameObject;
    [SerializeField] private ButtonTypes buttonType;
    private Button     button;
    public GameObject  ButtonGameObject {get{return buttonGameObject;} private set{buttonGameObject = value;}}
    public ButtonTypes ButtonType       {get{return buttonType;}       private set{buttonType       = value;}}
    public Button      Button           {get{return button;}                   set{button           = value;}}
}

public class ButtonsController : MonoBehaviour {
    private ScenesController scenesController;
    [SerializeField] private List<ButtonAtributes> buttonAttributes = new List<ButtonAtributes>();


    public List<ButtonAtributes> ButtonAttributes { get{return buttonAttributes;} private set{buttonAttributes = value;} }
    // Start is called before the first frame update
    void Awake() {
        scenesController = FindObjectOfType<ScenesController>();
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
