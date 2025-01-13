using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScenesController : Singleton<ScenesController> {
    private ScenesData D;
    //|--------------------------------------------------------------|
    //|                                                              |
    //| This is the only script that should have an Awake function!! |
    //|                                                              |
    //|--------------------------------------------------------------|

    //|--------------------------------------------------------------|
    //|                                                              |
    //| This is the only script that should have an Awake function!! |
    //|                                                              |
    //|--------------------------------------------------------------|
    public override IEnumerator Initialize() {
        yield return null;
    }

    new public void Awake() {
        D = ScenesData.Instance;
        SceneManager.sceneLoaded += OnInitialSceneLoad;
        StartCoroutine(StartSceneLoad());
    }

    private void OnInitialSceneLoad(Scene scene, LoadSceneMode mode) {
        D.initialFinishedLoading = true;
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        D.sceneFinishedLoading = true;
    }

    public void TryLoadScene(LoadSceneTypes loadSceneType, int levelNumber = 0) {
        int buildIndex;
        switch(loadSceneType) {
            case LoadSceneTypes.Menu:
                buildIndex = 1;
                break;
            case LoadSceneTypes.LevelSelector:
                buildIndex = 2;
                break;
            case LoadSceneTypes.Level:
                buildIndex = levelNumber + Constants.firstLevelBuidIndex - 1;
                break;
            case LoadSceneTypes.NextLevel:
                buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
                break;
            case LoadSceneTypes.PreviousLevel:
                buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
                break;
            default:
                buildIndex = 1;
                break;
        }

        if(buildIndex >= SceneManager.sceneCountInBuildSettings || buildIndex < 0) { 
            Debug.LogWarning($"No scene at buildIndex {buildIndex}"); 
            return; 
        }
        ScriptInitializationGlobal.Instance.SetUpdate(false);
        D.sceneFinishedLoading = false;
        D.animationIsFinished = false;
        StartCoroutine(LoadLevelCoroutine(buildIndex));
    }


    private IEnumerator LoadLevelCoroutine(int buildIndex) {
        //Play animation.
        D.crossFadeTransition.SetTrigger("StartCrossFade");

        //Wait for the crossfade to finish (all black screen) to load in the next scene.
        yield return new WaitForSeconds(D.crossFadeAnimationStartDuration);

        SceneManager.LoadScene(buildIndex);

        //Wait until the scene is finished loading to continue.
        yield return new WaitUntil(() => D.sceneFinishedLoading);
        
        //Initialize all necessary scripts
        StartCoroutine(ScriptInitializationGlobal.Instance.InitializeAll());
        yield return new WaitUntil(() => ScriptInitializationGlobal.Instance.FinshedInitialization);

        //Renew Resizes
        StartCoroutine(ResizeGlobal.Instance.RenewAll());

        //Load the game
        DataPersistenceManager.Instance.LoadGame();

        /*
        if(MenuInitializerGlobal.Instance != null) {
            yield return new WaitUntil(() => MenuInitializerGlobal.Instance.finishedWithAllTasks);
            yield return new WaitUntil(() => MenuInitializerGlobal.Instance.allButtonsLoaded);
        }
        else if(LevelSelectorInitializerGlobal.Instance != null) {
            yield return new WaitUntil(() => LevelSelectorInitializerGlobal.Instance.finishedWithAllTasks);
            yield return new WaitUntil(() => LevelSelectorInitializerGlobal.Instance.allButtonsLoaded);
        }
        else if(LevelInitializerGlobal.Instance != null) {
            yield return new WaitUntil(() => LevelInitializerGlobal.Instance.finishedWithAllTasks);
            yield return new WaitUntil(() => LevelInitializerGlobal.Instance.allButtonsLoaded);
        }
        */
        
        //End the crossfade transition.
        D.crossFadeTransition.SetTrigger("EndCrossFade");
        DebugC.Instance?.Log("Is finished with all tasks. Ending Fade.");

        //Set animationIsFinished to true after the animation is finished.
        yield return new WaitForSeconds(D.crossFadeAnimationEndDuration);
        D.animationIsFinished = true;
        DebugC.Instance?.Log("Animation ended.");
    }

    private IEnumerator StartSceneLoad() {
        yield return new WaitUntil(() => D.initialFinishedLoading);

        //Prerequisite for the rest of scene loads, does not contribute to this specific function
        SceneManager.sceneLoaded += OnSceneLoad;
        
        //Initialize all necessary scripts
        StartCoroutine(ScriptInitializationGlobal.Instance.InitializeAll());
        yield return new WaitUntil(() => ScriptInitializationGlobal.Instance.FinshedInitialization);

        //Renew Resizes
        StartCoroutine(ResizeGlobal.Instance.RenewAll());


        SceneManager.LoadScene(Constants.menuBuildIndex);
        StartCoroutine(MenuSceneLoad());
    }

    private IEnumerator MenuSceneLoad() {
        yield return new WaitUntil(() => D.sceneFinishedLoading);

        //Initialize all necessary scripts
        StartCoroutine(ScriptInitializationGlobal.Instance.InitializeAll());
        yield return new WaitUntil(() => ScriptInitializationGlobal.Instance.FinshedInitialization);

        //Renew Resizes
        StartCoroutine(ResizeGlobal.Instance.RenewAll());

        //Load the game
        DataPersistenceManager.Instance.LoadGame();



        /*
        if(MenuInitializerGlobal.Instance != null) {
            yield return new WaitUntil(() => MenuInitializerGlobal.Instance.finishedWithAllTasks);
            yield return new WaitUntil(() => MenuInitializerGlobal.Instance.allButtonsLoaded);
        }
        else if(LevelSelectorInitializerGlobal.Instance != null) {
            yield return new WaitUntil(() => LevelSelectorInitializerGlobal.Instance.finishedWithAllTasks);
            yield return new WaitUntil(() => LevelSelectorInitializerGlobal.Instance.allButtonsLoaded);
        }
        else if(LevelInitializerGlobal.Instance != null) {
            yield return new WaitUntil(() => LevelInitializerGlobal.Instance.finishedWithAllTasks);
            yield return new WaitUntil(() => LevelInitializerGlobal.Instance.allButtonsLoaded);
        }
        */
        D.crossFadeTransition.SetTrigger("InitialCrossFade");
        DebugC.Instance?.Log("Is finished with all tasks. Ending Fade. ");
        yield return new WaitForSeconds(D.crossFadeAnimationEndDuration);
        D.animationIsFinished = true;
    }



    public void OnPressEnterLevelButton(int levelNumber) {
        DataPersistenceManager.Instance.SaveGame();
        TryLoadScene(LoadSceneTypes.Level, levelNumber);
    }
    public void OnPressEnterNextLevelButton() {
        DataPersistenceManager.Instance.SaveGame();
        TryLoadScene(LoadSceneTypes.NextLevel);
    }
    public void OnPressEnterPreviousLevelButton() {
        DataPersistenceManager.Instance.SaveGame();
        TryLoadScene(LoadSceneTypes.PreviousLevel);
    }

    public void OnPressEnterLevelSelectorButton() {
        DataPersistenceManager.Instance.SaveGame();
        TryLoadScene(LoadSceneTypes.LevelSelector);
    }
    public void OnPressEnterMenuButton() {
        DataPersistenceManager.Instance.SaveGame();
        TryLoadScene(LoadSceneTypes.Menu);
    }
}

