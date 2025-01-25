using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PrefabIdentifierBase : ScriptInitializerBase {
    public override IEnumerator Initialize() {
        yield return null;
    }
    [Tooltip("Modify the id here within the prefab.")]
    [SerializeField] [Min(0)] int prefabId;
    public int PrefabId {get {return prefabId;} private set {}}
}
