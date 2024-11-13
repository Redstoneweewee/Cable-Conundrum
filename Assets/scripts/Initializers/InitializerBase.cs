using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerBase : MonoBehaviour {
    public bool finishedWithAllTasks = false;
    public bool allButtonsLoaded = false;
    [HideInInspector] public ButtonsGlobal    buttonsGlobal;
    [HideInInspector] public ScenesController scenesController;
    [HideInInspector] public SettingsGlobal   settingsGlobal;

    [SerializeField]  public ButtonAttributes[] buttonAttributes;

    public void Start() {
        StartCoroutine(WaitToInitializeButtons());
    }

    private IEnumerator WaitToInitializeButtons() {
        yield return new WaitUntil(() => finishedWithAllTasks);
        Debug.Log("buttons waited");
        buttonsGlobal    = FindObjectOfType<ButtonsGlobal>();
        scenesController = FindObjectOfType<ScenesController>();
        settingsGlobal   = FindObjectOfType<SettingsGlobal>();
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
                    buttonsGlobal.SubscribeToButton(buttonAttribute.button, scenesController.OnPressEnterLevelButton);
                    break;
                case ButtonTypes.EnterMenu:
                    buttonsGlobal.SubscribeToButton(buttonAttribute.button, scenesController.OnPressEnterMenuButton);
                    break;
                case ButtonTypes.EnterSettings:
                    buttonsGlobal.SubscribeToButton(buttonAttribute.button, settingsGlobal.OnPressEnterSettingsButton);
                    break;
                case ButtonTypes.ExitSettings:
                    Debug.Log($"settingsGlobal: {settingsGlobal.name} ");
                    buttonsGlobal.SubscribeToButton(buttonAttribute.button, settingsGlobal.OnPressExitSettingsButton);
                    break;
            }
        }

        foreach(GameObjectActivity activity in gameObjectActivities) {
            activity.gameObject.SetActive(activity.isInitiallyActive);
            Debug.Log($"set {activity.gameObject.name} to {activity.isInitiallyActive}");
        }
        AllButtonsLoaded();
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
