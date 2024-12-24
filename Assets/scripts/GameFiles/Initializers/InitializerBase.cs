using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerBase : MonoBehaviour {
    public bool finishedWithAllTasks = false;
    public bool allButtonsLoaded = false;
    [HideInInspector] public ScenesController           scenesController;
    [HideInInspector] public ExitGameConfirmationGlobal exitGameConfirmationGlobal;
    [HideInInspector] public SettingsGlobal             settingsGlobal;
    [HideInInspector] public TutorialController         tutorialController;
    [HideInInspector] public TutorialData               tutorialData;
    [HideInInspector] public CreditsGlobal              creditsGlobal;
    [HideInInspector] public ControlsController         controlsController;
    [HideInInspector] public SoundsController           soundsController;
    [HideInInspector] public ResizeGlobal               resizeGlobal;

    [SerializeField]  public ButtonsAttributes[] buttonsAttributes;

    public void Awake() {
        
    }

    public void Start() {
        StartCoroutine(WaitToInitializeButtons());
    }

    private IEnumerator WaitToInitializeButtons() {
        yield return new WaitUntil(() => finishedWithAllTasks);
        scenesController = FindFirstObjectByType<ScenesController>();
        settingsGlobal   = FindFirstObjectByType<SettingsGlobal>(FindObjectsInactive.Include);
        exitGameConfirmationGlobal = FindFirstObjectByType<ExitGameConfirmationGlobal>(FindObjectsInactive.Include);
        tutorialController = FindFirstObjectByType<TutorialController>();
        tutorialData       = FindFirstObjectByType<TutorialData>();
        creditsGlobal      = FindFirstObjectByType<CreditsGlobal>();
        controlsController = FindFirstObjectByType<ControlsController>();
        soundsController   = FindFirstObjectByType<SoundsController>();
        resizeGlobal       = FindFirstObjectByType<ResizeGlobal>();
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

        controlsController.Initialize();
        soundsController.InitializeSliders();
        InitializeButtons();

        // |--------------------------------------------------------------------------|
        // |--------------------------------------------------------------------------|
        // |-------------- WARNING - Videos cannot load without WIFI!! ---------------|
        // |--------------------------------------------------------------------------|
        // |--------------------------------------------------------------------------|
        
        if(!tutorialData.isInitialized) { 
            //StartCoroutine(tutorialController.Initialize());
            
            try { StartCoroutine(tutorialController.Initialize()); Debug.Log("initializing tutorial");}
            catch(Exception e) {
                Debug.LogWarning("tutorial was unable to load. Error: "+e);
            }
            
        }
        yield return new WaitUntil(() => tutorialData.isInitialized);
        
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
                    Utilities.SubscribeToButton(buttonsAttribute.button, scenesController.OnPressEnterLevelButton, buttonsAttribute.levelIndex);
                    break;
                case ButtonTypes.NextLevel:
                    Utilities.SubscribeToButton(buttonsAttribute.button, scenesController.OnPressEnterNextLevelButton);
                    break;
                case ButtonTypes.PreviousLevel:
                    Utilities.SubscribeToButton(buttonsAttribute.button, scenesController.OnPressEnterPreviousLevelButton);
                    break;
                case ButtonTypes.EnterLevelSelector:
                    Utilities.SubscribeToButton(buttonsAttribute.button, scenesController.OnPressEnterLevelSelectorButton);
                    break;
                case ButtonTypes.EnterMenu:
                    Utilities.SubscribeToButton(buttonsAttribute.button, scenesController.OnPressEnterMenuButton);
                    break;
                case ButtonTypes.EnterSettings:
                    Utilities.SubscribeToButton(buttonsAttribute.button, settingsGlobal.OnPressEnterSettingsButton);
                    break;
                case ButtonTypes.ExitSettings:
                    Utilities.SubscribeToButton(buttonsAttribute.button, settingsGlobal.OnPressExitSettingsButton);
                    break;
                case ButtonTypes.EnterExitConfirmation:
                    Utilities.SubscribeToButton(buttonsAttribute.button, exitGameConfirmationGlobal.OnPressEnterExitConfirmationButton);
                    break;
                case ButtonTypes.ExitExitConfirmation:
                    Utilities.SubscribeToButton(buttonsAttribute.button, exitGameConfirmationGlobal.OnPressExitExitConfirmationButton);
                    break;
                case ButtonTypes.ExitGame:
                    Utilities.SubscribeToButton(buttonsAttribute.button, exitGameConfirmationGlobal.OnPressExitGameButton);
                    break;
                case ButtonTypes.EnterTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, tutorialController.OnPressEnterTutorialPageButton);
                    break;
                case ButtonTypes.ExitTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, tutorialController.OnPressExitTutorialPageButton);
                    break;
                case ButtonTypes.NextTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, tutorialController.OnPressNextTutorialPageButton);
                    break;
                case ButtonTypes.PreviousTutorialPage:
                    Utilities.SubscribeToButton(buttonsAttribute.button, tutorialController.OnPressPreviousTutorialPageButton);
                    break;
                case ButtonTypes.EnterCredits:
                    Utilities.SubscribeToButton(buttonsAttribute.button, creditsGlobal.OnPressEnterCreditsButton);
                    break;
                case ButtonTypes.ExitCredits:
                    Utilities.SubscribeToButton(buttonsAttribute.button, creditsGlobal.OnPressExitCreditsButton);
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
        DebugC.Get()?.Log("set finishedWithAllTasks to True");
    }
    public void AllButtonsLoaded() {
        allButtonsLoaded = true;
        DebugC.Get()?.Log("set allButtonsLoaded to True");
    }
}
