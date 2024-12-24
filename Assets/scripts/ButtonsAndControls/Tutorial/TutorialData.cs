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
    /*[HideInInspector] */[Range(0, 1)] public int useNormalVideoPlayers = 1;
    [Header("videoPlayers contains the URL parent as the first element and \nNormal parent as the second element.")]
    [SerializeField] public GameObject[] videoPlayerParents = new GameObject[2];
    /*[HideInInspector] */public TutorialVideoAttributes[] videoPlayers;
    /*[HideInInspector] */public bool isInitialized = false;
    /*[HideInInspector] */public bool initialVideoInitialized = false;
    [HideInInspector] public int currentPageIndex = 0;


    void Awake() {
        tutorialController = Utilities.TryGetComponent<TutorialController>(gameObject);
    }
}
