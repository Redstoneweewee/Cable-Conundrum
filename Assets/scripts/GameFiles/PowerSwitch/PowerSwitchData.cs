using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSwitchData : MonoBehaviour {
    [HideInInspector] public PowerSwitchController     powerSwitchController;
    [HideInInspector] public DebugC                    debugC;
    [HideInInspector] public WinningControllerGlobal   winningControllerGlobal;
    [SerializeField]  public GameObject                offVisual;
    [SerializeField]  public GameObject                onVisual;
    [HideInInspector] public ElectricalStripController electricalStripController;
    [HideInInspector] public IntersectionData          intersectionData;

    void Awake() {
        debugC = DebugC.Get();
        powerSwitchController     = Utilities.TryGetComponent<PowerSwitchController>(gameObject);
        winningControllerGlobal   = FindObjectOfType<WinningControllerGlobal>();
        electricalStripController = FindObjectOfType<ElectricalStripController>();
        intersectionData          = FindObjectOfType<IntersectionData>();
    }
}
