using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LoadSceneType { Next, Previous, Menu }

public class ScenesController : MonoBehaviour {
    [SerializeField] private Animator crossFadeTransition;
    [SerializeField] private float crossFadeAnimationStartDuration;
    [SerializeField] private float crossFadeAnimationEndDuration;
    [SerializeField] private bool sceneFinishedLoading = false;
    [SerializeField] private bool animationIsFinished = false;



    void Awake() {
        ScenesController[] scenesControllers = FindObjectsOfType<ScenesController>();
        if(scenesControllers.Length > 1) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;

        StartCoroutine(InitialSceneLoad());
    }



    void Update() {
        if(!animationIsFinished) { return; }

        //if(animationIsFinished && Input.GetMouseButtonDown(0)) {
        //    LoadLevel(LoadLevelType.Next);
        //}
        //else if(animationIsFinished && Input.GetMouseButtonDown(1)) {
        //    LoadLevel(LoadLevelType.Previous);
        //}
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        sceneFinishedLoading = true;
    }

    public void LoadLevel(LoadSceneType loadSceneType) {
        int nextBuildIndex = SceneManager.GetActiveScene().buildIndex;
        switch(loadSceneType) {
            case LoadSceneType.Menu:
                nextBuildIndex = 0;
                break;
            case LoadSceneType.Next:
                nextBuildIndex++;
                break;
            case LoadSceneType.Previous:
                nextBuildIndex--;
                break;
        }

        if(nextBuildIndex >= SceneManager.sceneCountInBuildSettings || nextBuildIndex < 0) { 
            Debug.LogWarning($"No scene at buildIndex {nextBuildIndex}"); 
            return; 
        }
        sceneFinishedLoading = false;
        animationIsFinished = false;
        StartCoroutine(LoadLevelCoroutine(nextBuildIndex));
    }


    private IEnumerator LoadLevelCoroutine(int buildIndex) {
        //Play animation.
        crossFadeTransition.SetTrigger("StartCrossFade");

        //Wait for the crossfade to finish (all black screen) to load in the next scene.
        yield return new WaitForSeconds(crossFadeAnimationStartDuration);
        SceneManager.LoadScene(buildIndex);

        //Wait until the scene is finished loading to continue.
        yield return new WaitUntil(() => sceneFinishedLoading);
        
        //If the scene has a LevelStart component, wait until everything is loaded before ending the crossfade transition.
        if(FindObjectOfType<LevelStart>()) {
            yield return new WaitUntil(() => FindObjectOfType<LevelStart>().isFinishedWithAllTasks);
        }
        
        //End the crossfade transition.
        crossFadeTransition.SetTrigger("EndCrossFade");
        Debug.Log("Is finished with all tasks. Ending Fade.");

        //Set animationIsFinished to true after the animation is finished.
        yield return new WaitForSeconds(crossFadeAnimationEndDuration);
        animationIsFinished = true;
    }


    private IEnumerator InitialSceneLoad() {
        yield return new WaitForSeconds(crossFadeAnimationEndDuration);
        animationIsFinished = true;
    }






    public void OnPressStartButton() {
        //for now just go to scene 1, which is the "next" level
        LoadLevel(LoadSceneType.Next);
    }


    public void OnPressExitToMenuButton() {
        LoadLevel(LoadSceneType.Menu);
    }



}

