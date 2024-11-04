using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonAtributes {
    [SerializeField] private GameObject buttonGameObject;
    [SerializeField] private ButtonTypes buttonType;
    private Button     button;
    public GameObject  ButtonGameObject {get{return buttonGameObject;} private set{buttonGameObject = value;}}
    public ButtonTypes ButtonType       {get{return buttonType;}       private set{buttonType       = value;}}
    public Button      Button           {get{return button;}                   set{button           = value;}}
}
