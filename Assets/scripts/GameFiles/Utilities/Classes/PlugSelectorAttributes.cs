using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlugSelectorAtributes {
    private GameObject buttonGameObject;
    [SerializeField] private GameObject plugPrefab;
    [SerializeField] private GameObject plugImage;
    [SerializeField] private PlugSelectorTypes type;
    private Button   button;
    public GameObject        ButtonGameObject {get{return buttonGameObject;}         set{buttonGameObject = value;}}
    public GameObject        PlugPrefab       {get{return plugPrefab;}       private set{plugPrefab       = value;}}
    public GameObject        PlugImage        {get{return plugImage;}        private set{plugImage        = value;}}
    public PlugSelectorTypes Type             {get{return type;}             private set{type             = value;}}
    public Button            Button           {get{return button;}                   set{button           = value;}}
}
