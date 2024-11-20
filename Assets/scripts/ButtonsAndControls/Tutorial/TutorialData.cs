using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialData : MonoBehaviour {
    [SerializeField] public GameObject tutorialCanvas;
    [SerializeField] public GameObject videoDisplay;
    [SerializeField] public GameObject description;
    [SerializeField] public GameObject nextTutorialPageButton;
    [SerializeField] public GameObject previousTutorialPageButton;
    [HideInInspector] public TutorialController tutorialController;
    public TutorialVideoAttributes[] videoPlayers;
    [SerializeField] public bool isInitialized = false;
    [HideInInspector] public bool initialVideoInitialized = false;
    [HideInInspector] public int currentPageIndex = 0;


    void Awake() {
        tutorialController = Utilities.TryGetComponent<TutorialController>(gameObject);
    }
}
