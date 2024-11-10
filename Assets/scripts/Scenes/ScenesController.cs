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



    void Update() {
        if(!D.animationIsFinished) { return; }

        //if(animationIsFinished && Input.GetMouseButtonDown(0)) {
        //    LoadLevel(LoadLevelType.Next);
        //}
        //else if(animationIsFinished && Input.GetMouseButtonDown(1)) {
        //    LoadLevel(LoadLevelType.Previous);
        //}
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        D.sceneFinishedLoading = true;
    }

    public void LoadLevel(LoadSceneTypes loadSceneType) {
        int nextBuildIndex = SceneManager.GetActiveScene().buildIndex;
        switch(loadSceneType) {
            case LoadSceneTypes.Menu:
                nextBuildIndex = 0;
                break;
            case LoadSceneTypes.Next:
                nextBuildIndex++;
                break;
            case LoadSceneTypes.Previous:
                nextBuildIndex--;
                break;
        }

        if(nextBuildIndex >= SceneManager.sceneCountInBuildSettings || nextBuildIndex < 0) { 
            Debug.LogWarning($"No scene at buildIndex {nextBuildIndex}"); 
            return; 
        }
        D.sceneFinishedLoading = false;
        D.animationIsFinished = false;
        StartCoroutine(LoadLevelCoroutine(nextBuildIndex));
    }


    private IEnumerator LoadLevelCoroutine(int buildIndex) {
        //Play animation.
        D.crossFadeTransition.SetTrigger("StartCrossFade");

        //Wait for the crossfade to finish (all black screen) to load in the next scene.
        yield return new WaitForSeconds(D.crossFadeAnimationStartDuration);
        SceneManager.LoadScene(buildIndex);

        //Wait until the scene is finished loading to continue.
        yield return new WaitUntil(() => D.sceneFinishedLoading);
        
        //If the scene has a LevelStart component, wait until everything is loaded before ending the crossfade transition.
        if(FindObjectOfType<LevelStartGlobal>()) {
            yield return new WaitUntil(() => FindObjectOfType<LevelStartGlobal>().isFinishedWithAllTasks);
        }
        
        //End the crossfade transition.
        D.crossFadeTransition.SetTrigger("EndCrossFade");
        Debug.Log("Is finished with all tasks. Ending Fade.");

        //Set animationIsFinished to true after the animation is finished.
        yield return new WaitForSeconds(D.crossFadeAnimationEndDuration);
        D.animationIsFinished = true;
    }


    private IEnumerator InitialSceneLoad() {
        yield return new WaitForSeconds(D.crossFadeAnimationEndDuration);
        D.animationIsFinished = true;
    }






    public void OnPressEnterLevelButton() {
        //for now just go to scene 1, which is the "next" level
        LoadLevel(LoadSceneTypes.Next);
    }

    public void OnPressEnterMenuButton() {
        LoadLevel(LoadSceneTypes.Menu);
    }



}

