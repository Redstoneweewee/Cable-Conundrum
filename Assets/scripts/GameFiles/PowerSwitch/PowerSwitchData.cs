using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSwitchData : MonoBehaviour {
    [HideInInspector] public PowerSwitchController     powerSwitchController;
    [HideInInspector] public LevelInitializerGlobal    levelInitializerGlobal;
    [HideInInspector] public WinningControllerGlobal   winningControllerGlobal;
    [SerializeField]  public GameObject                offVisual;
    [SerializeField]  public GameObject                onVisual;
    [HideInInspector] public ElectricalStripController electricalStripController;
    [HideInInspector] public IntersectionData          intersectionData;

    void Awake() {
        powerSwitchController     = Utilities.TryGetComponent<PowerSwitchController>(gameObject);
        levelInitializerGlobal    = FindFirstObjectByType<LevelInitializerGlobal>();
        winningControllerGlobal   = FindFirstObjectByType<WinningControllerGlobal>();
        electricalStripController = FindFirstObjectByType<ElectricalStripController>();
        intersectionData          = FindFirstObjectByType<IntersectionData>();
    }
}
