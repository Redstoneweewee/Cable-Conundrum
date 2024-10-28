using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ElectricalStripValues : MonoBehaviour {

    private ElectricalStripController electricalStripController;
    private ElectricalStripSizeController electricalStripSizeController;
    public ElectricalStripController ElectricalStripController         { get{ return electricalStripController;     } set{ electricalStripController     = value; } }
    public ElectricalStripSizeController ElectricalStripSizeController { get{ return electricalStripSizeController; } set{ electricalStripSizeController = value; } }

    void Awake() {
        electricalStripController = GetComponent<ElectricalStripController>();
        electricalStripSizeController = GetComponent<ElectricalStripSizeController>();
    }

}
