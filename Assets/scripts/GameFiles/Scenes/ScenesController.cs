using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScenesController : MonoBehaviour {
    private ScenesData D;

    void Awake() {
        D = Utilities.TryGetComponent<ScenesData>(gameObject);

        SceneManager.sceneLoaded += OnSceneLoad;
        StartCoroutine(InitialSceneLoad());
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        D.sceneFinishedLoading = true;
    }

    public void TryLoadScene(LoadSceneTypes loadSceneType, int levelIndex = 0) {
        int buildIndex;
        switch(loadSceneType) {
            case LoadSceneTypes.Menu:
                buildIndex = 0;
                break;
            case LoadSceneTypes.LevelSelector:
                buildIndex = 1;
                break;
            case LoadSceneTypes.Level:
                buildIndex = levelIndex + Constants.firstLevelBuidIndex - 1;
                break;
            case LoadSceneTypes.NextLevel:
                buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
                break;
            case LoadSceneTypes.PreviousLevel:
                buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
                break;
            default:
                buildIndex = 0;
                break;
        }

        if(buildIndex >= SceneManager.sceneCountInBuildSettings || buildIndex < 0) { 
            Debug.LogWarning($"No scene at buildIndex {buildIndex}"); 
            return; 
        }
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
        
        //Renew Resizes
        FindFirstObjectByType<ResizeGlobal>().RenewAll();
        Debug.Log("AAAAAAAAAAAAAAAAAAAA renewed resizes");

        //Load the game
        DataPersistenceManager.instance.LoadGame();

        //If the scene has a LevelStart component, wait until everything is loaded before ending the crossfade transition.
        if(FindFirstObjectByType<InitializerBase>()) {
            yield return new WaitUntil(() => FindFirstObjectByType<InitializerBase>().finishedWithAllTasks);
            yield return new WaitUntil(() => FindFirstObjectByType<InitializerBase>().allButtonsLoaded);
        }
        
        //End the crossfade transition.
        D.crossFadeTransition.SetTrigger("EndCrossFade");
        DebugC.Get()?.Log("Is finished with all tasks. Ending Fade.");

        //Set animationIsFinished to true after the animation is finished.
        yield return new WaitForSeconds(D.crossFadeAnimationEndDuration);
        D.animationIsFinished = true;
        DebugC.Get()?.Log("Animation ended.");
    }


    private IEnumerator InitialSceneLoad() {
        yield return new WaitUntil(() => FindFirstObjectByType<InitializerBase>().finishedWithAllTasks);
        yield return new WaitUntil(() => FindFirstObjectByType<InitializerBase>().allButtonsLoaded);
        D.crossFadeTransition.SetTrigger("InitialCrossFade");
        DebugC.Get()?.Log("Is finished with all tasks. Ending Fade. ");
        yield return new WaitForSeconds(D.crossFadeAnimationEndDuration);
        D.animationIsFinished = true;
    }



    public void OnPressEnterLevelButton(int levelIndex) {
        DataPersistenceManager.instance.SaveGame();
        TryLoadScene(LoadSceneTypes.Level, levelIndex);
    }
    public void OnPressEnterNextLevelButton() {
        DataPersistenceManager.instance.SaveGame();
        TryLoadScene(LoadSceneTypes.NextLevel);
    }
    public void OnPressEnterPreviousLevelButton() {
        DataPersistenceManager.instance.SaveGame();
        TryLoadScene(LoadSceneTypes.PreviousLevel);
    }

    public void OnPressEnterLevelSelectorButton() {
        DataPersistenceManager.instance.SaveGame();
        TryLoadScene(LoadSceneTypes.LevelSelector);
    }
    public void OnPressEnterMenuButton() {
        DataPersistenceManager.instance.SaveGame();
        TryLoadScene(LoadSceneTypes.Menu);
    }
}

