using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonAttributes : MonoBehaviour {
    [SerializeField]  public ButtonTypes buttonType;
    [Tooltip("Level will only be used if buttonType is set to EnterLevel.")]
    [SerializeField]  public int         levelIndex = 0;
    [HideInInspector] public Button      button;
    [HideInInspector] public GameObject  buttonGameObject;
    
    void Awake() {
        buttonGameObject = gameObject;
        button = Utilities.TryGetComponentInChildren<Button>(gameObject);
    }
}
