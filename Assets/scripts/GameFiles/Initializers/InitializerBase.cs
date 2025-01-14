using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InitializerBase<T> : Singleton<T> where T : MonoBehaviour {
    //public bool finishedWithAllTasks = false;
    //public bool allButtonsLoaded = false;
    [SerializeField]  public ButtonsAttributes[] buttonsAttributes;

    //ublic override void OnAwake() { }
    public override IEnumerator Initialize() {
        InitializeItems();
        yield return null;
    }

    private void InitializeItems() {

        //ControlsController.Instance.InitializeOld();
        InitializeButtons();

        /*
        if(!TutorialData.Instance.isInitialized) { 
            //StartCoroutine(tutorialController.Initialize());
            
            try { StartCoroutine(TutorialController.Instance.InitializeOld()); Debug.Log("initializing tutorial");}
            catch(Exception e) {
                Debug.LogWarning("tutorial was unable to load. Error: "+e);
            }
            
        }
        yield return new WaitUntil(() => TutorialData.Instance.isInitialized);
        */
        //AllButtonsLoaded();
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


    //private void EndInitialization(GameObjectActivity[] gameObjectActivities) {
    //    foreach(GameObjectActivity activity in gameObjectActivities) {
    //        activity.gameObject.SetActive(activity.isInitiallyActive);
    //    }
    //}

    public IEnumerator SetMenuButton(bool active) {
        //yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonsAttributes buttonAttribute in buttonsAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterMenu) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
        Debug.Log("Set menu button to "+active);
        yield return null;
    }
    public IEnumerator SetLevelSelectorButton(bool active) {
        //yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonsAttributes buttonAttribute in buttonsAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterLevelSelector) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
        Debug.Log("Set level selector button to "+active);
        yield return null;
    }
    public IEnumerator SetTutorialHelpButton(bool active) {
        //yield return new WaitUntil(() => allButtonsLoaded);
        foreach(ButtonsAttributes buttonAttribute in buttonsAttributes) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterTutorialPage) {
                buttonAttribute.button.gameObject.SetActive(active);
            }
        }
        Debug.Log("Set tutorial button to "+active);
        yield return null;
    }


    //public void FinishedWithAllTasks() {
    //    finishedWithAllTasks = true;
    //    DebugC.Instance?.Log("set finishedWithAllTasks to True");
    //}
    //public void AllButtonsLoaded() {
    //    allButtonsLoaded = true;
    //    DebugC.Instance?.Log("set allButtonsLoaded to True");
    //}
}
