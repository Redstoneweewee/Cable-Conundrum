using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonsAttributes : ScriptInitializerBase {
    [SerializeField]  public ButtonsHandler buttonsHandler;
    [SerializeField]  public ButtonTypes buttonType;
    [Tooltip("Level will only be used if buttonType is set to EnterLevel.")]
    [SerializeField]  public int         levelIndex = 0;
    [HideInInspector] public Button      button;
    [HideInInspector] public GameObject  buttonGameObject;
    
    public override IEnumerator Initialize() {
        buttonGameObject = gameObject;
        buttonsHandler = Utilities.TryGetComponent<ButtonsHandler>(gameObject);
        button = Utilities.TryGetComponentInChildren<Button>(gameObject);
        yield return null;
    }
}
