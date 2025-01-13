using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InitializerBase<T> : Singleton<T> where T : MonoBehaviour {
    public bool finishedWithAllTasks = false;
    public bool allButtonsLoaded = false;
    [SerializeField]  public ButtonsAttributes[] buttonsAttributes;

    //ublic override void OnAwake() { }
    public override IEnumerator Initialize() {
        StartCoroutine(WaitToInitializeButtons());
        yield return null;
    }

    private IEnumerator WaitToInitializeButtons() {
        yield return new WaitUntil(() => finishedWithAllTasks);
        StartCoroutine(InitializeItems());
    }

    private IEnumerator InitializeItems() {
        /*
        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        GameObjectActivity[] gameObjectActivities = new GameObjectActivity[allTransforms.Length];
        for(int i=0; i<allTransforms.Length; i++) {
            gameObjectActivities[i] = new GameObjectActivity(allTransforms[i].gameObject, allTransforms[i].gameObject.activeSelf);
        }
        foreach(Transform transform in allTransforms) {
            transform.gameObject.SetActive(true);
        }
        */

        ControlsController.Instance.InitializeOld();
        SoundsController.Instance.InitializeSliders();
        InitializeButtons();

        // |--------------------------------------------------------------------------|
        // |--------------------------------------------------------------------------|
        // |-------------- WARNING - Videos cannot load without WIFI!! ---------------|
        // |--------------------------------------------------------------------------|
        // |--------------------------------------------------------------------------|
        
        if(!TutorialData.Instance.isInitialized) { 
            //StartCoroutine(tutorialController.Initialize());
            
            try { StartCoroutine(TutorialController.Instance.InitializeOld()); Debug.Log("initializing tutorial");}
            catch(Exception e) {
                Debug.LogWarning("tutorial was unable to load. Error: "+e);
            }
            
        }
        yield return new WaitUntil(() => TutorialData.Instance.isInitialized);
        
        //EndInitialization(gameObjectActivities);
        //yield return null;
        AllButtonsLoaded();
    }


    private void InitializeButtons() {


        buttonsAttributes = FindObjectsByType<ButtonsAttributes>(FindObjectsSortMode.None);
        foreach(ButtonsAttributes buttonsAttribute in buttonsAttributes) {
            buttonsAttribute.button.onClick.RemoveAllListeners();
            Utilities.SubscribeToButton(buttonsAttribute.button, buttonsAttribute.buttonsHandler.OnPressButton);
            switch(buttonsAttribute.buttonType) {
                case ButtonTypes.EnterLevel:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ScenesController.Instance.OnPressEnterLevelButton, buttonsAttribute.levelIndex);
                    break;
                case ButtonTypes.NextLevel:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ScenesController.Instance.OnPressEnterNextLevelButton);
                    break;
                case ButtonTypes.PreviousLevel:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ScenesController.Instance.OnPressEnterPreviousLevelButton);
                    break;
                case ButtonTypes.EnterLevelSelector:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ScenesController.Instance.OnPressEnterLevelSelectorButton);
                    break;
                case ButtonTypes.EnterMenu:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ScenesController.Instance.OnPressEnterMenuButton);
                    break;
                case ButtonTypes.EnterSettings:
                    Utilities.SubscribeToButton(buttonsAttribute.button, SettingsGlobal.Instance.OnPressEnterSettingsButton);
                    break;
                case ButtonTypes.ExitSettings:
                    Utilities.SubscribeToButton(buttonsAttribute.button, SettingsGlobal.Instance.OnPressExitSettingsButton);
                    break;
                case ButtonTypes.EnterExitConfirmation:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ExitGameConfirmationGlobal.Instance.OnPressEnterExitConfirmationButton);
                    break;
                case ButtonTypes.ExitExitConfirmation:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ExitGameConfirmationGlobal.Instance.OnPressExitExitConfirmationButton);
                    break;
                case ButtonTypes.ExitGame:
                    Utilities.SubscribeToButton(buttonsAttribute.button, ExitGameConfirmationGlobal.Instance.OnPressExitGameButton);
                    break;
                case ButtonTypes.EnterTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, TutorialController.Instance.OnPressEnterTutorialPageButton);
                    break;
                case ButtonTypes.ExitTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, TutorialController.Instance.OnPressExitTutorialPageButton);
                    break;
                case ButtonTypes.NextTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, TutorialController.Instance.OnPressNextTutorialPageButton);
                    break;
                case ButtonTypes.PreviousTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, TutorialController.Instance.OnPressPreviousTutorialPageButton);
                    break;
                case ButtonTypes.EnterCredits:
                    Utilities.SubscribeToButton(buttonsAttribute.button, CreditsGlobal.Instance.OnPressEnterCreditsButton);
                    break;
                case ButtonTypes.ExitCredits:
                    Utilities.SubscribeToButton(buttonsAttribute.button, CreditsGlobal.Instance.OnPressExitCreditsButton);
                    break;
                default:
                    Debug.LogError($"Undefined button type: {buttonsAttribute.buttonType}");
                    break;
            }
        }
    }


    private void EndInitialization(GameObjectActivity[] gameObjectActivities) {
        foreach(GameObjectActivity activity in gameObjectActivities) {
            activity.gameObject.SetActive(activity.isInitiallyActive);
        }
    }

    public IEnumerator SetMenuButton(bool active) {
        yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonsAttributes buttonAttribute in buttonsAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterMenu) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
    }
    public IEnumerator SetLevelSelectorButton(bool active) {
        yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonsAttributes buttonAttribute in buttonsAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterLevelSelector) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
    }
    public IEnumerator SetTutorialHelpButton(bool active) {
        yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonsAttributes buttonAttribute in buttonsAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterTutorialPage) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
    }


    public void FinishedWithAllTasks() {
        finishedWithAllTasks = true;
        DebugC.Instance?.Log("set finishedWithAllTasks to True");
    }
    public void AllButtonsLoaded() {
        allButtonsLoaded = true;
        DebugC.Instance?.Log("set allButtonsLoaded to True");
    }
}
