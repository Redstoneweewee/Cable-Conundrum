using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsData : MonoBehaviour {
    [HideInInspector] public SoundsController soundsController;
    [HideInInspector] public SoundsEditor soundsEditor;
    [SerializeField] public List<SoundsAttributes> soundEffects;
    [SerializeField] public List<SoundsAttributes> music;
    [Range(0, 1)] public float soundVolume = 1;
    [Range(0, 1)] public float musicVolume = 1;

    void Awake() {
        soundsController = Utilities.TryGetComponent<SoundsController>(gameObject);
        soundsEditor     = FindObjectOfType<SoundsEditor>();
    }
}
