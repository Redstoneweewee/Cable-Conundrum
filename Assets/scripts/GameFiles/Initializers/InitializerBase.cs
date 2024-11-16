using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerBase : MonoBehaviour {
    public bool finishedWithAllTasks = false;
    public bool allButtonsLoaded = false;
    [HideInInspector] public ScenesController  scenesController;
    [HideInInspector] public SettingsGlobal    settingsGlobal;

    [SerializeField]  public ButtonAttributes[] buttonAttributes;

    public void Start() {
        StartCoroutine(WaitToInitializeButtons());
    }

    private IEnumerator WaitToInitializeButtons() {
        yield return new WaitUntil(() => finishedWithAllTasks);
        Debug.Log("buttons waited");
        scenesController = FindObjectOfType<ScenesController>();
        settingsGlobal   = FindObjectOfType<SettingsGlobal>(true);
        InitializeButtons();
    }

    private void InitializeButtons() {
        Debug.Log("doing buttons stuff");

        Transform[] allTransforms = FindObjectsOfType<Transform>(true);
        GameObjectActivity[] gameObjectActivities = new GameObjectActivity[allTransforms.Length];
        for(int i=0; i<allTransforms.Length; i++) {
            gameObjectActivities[i] = new GameObjectActivity(allTransforms[i].gameObject, allTransforms[i].gameObject.activeSelf);
        }
        foreach(Transform transform in allTransforms) {
            transform.gameObject.SetActive(true);
        }

        buttonAttributes = FindObjectsOfType<ButtonAttributes>(true);
        foreach(ButtonAttributes buttonAttribute in buttonAttributes) {
            switch(buttonAttribute.buttonType) {
                case ButtonTypes.EnterLevel:
                    Utilities.SubscribeToButton(buttonAttribute.button, scenesController.OnPressEnterLevelButton, buttonAttribute.levelIndex);
                    break;
                case ButtonTypes.NextLevel:
                    Utilities.SubscribeToButton(buttonAttribute.button, scenesController.OnPressEnterNextLevelButton);
                    break;
                case ButtonTypes.PreviousLevel:
                    Utilities.SubscribeToButton(buttonAttribute.button, scenesController.OnPressEnterPreviousLevelButton);
                    break;
                case ButtonTypes.EnterLevelSelector:
                    Utilities.SubscribeToButton(buttonAttribute.button, scenesController.OnPressEnterLevelSelectorButton);
                    break;
                case ButtonTypes.EnterMenu:
                    Utilities.SubscribeToButton(buttonAttribute.button, scenesController.OnPressEnterMenuButton);
                    break;
                case ButtonTypes.EnterSettings:
                    Debug.Log("subscribed to enter settings");
                    Utilities.SubscribeToButton(buttonAttribute.button, settingsGlobal.OnPressEnterSettingsButton);
                    break;
                case ButtonTypes.ExitSettings:
                    Debug.Log($"settingsGlobal: {settingsGlobal.name} ");
                    Debug.Log("subscribed to exit  settings");
                    Utilities.SubscribeToButton(buttonAttribute.button, settingsGlobal.OnPressExitSettingsButton);
                    break;
            }
        }

        foreach(GameObjectActivity activity in gameObjectActivities) {
            activity.gameObject.SetActive(activity.isInitiallyActive);
            //Debug.Log($"set {activity.gameObject.name} to {activity.isInitiallyActive}");
        }
        AllButtonsLoaded();
    }


    public IEnumerator SetMenuButton(bool active) {
        yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonAttributes buttonAttribute in buttonAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterMenu) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
    }
    public IEnumerator SetLevelSelectorButton(bool active) {
        yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonAttributes buttonAttribute in buttonAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterLevelSelector) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
    }


    public void FinishedWithAllTasks() {
        finishedWithAllTasks = true;
        Debug.Log("set finishedWithAllTasks to True");
    }
    public void AllButtonsLoaded() {
        allButtonsLoaded = true;
        Debug.Log("set allButtonsLoaded to True");
    }
}
