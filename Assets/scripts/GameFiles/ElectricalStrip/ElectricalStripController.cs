using System;
using System.Collections;
using UnityEngine;

public class ElectricalStripController : Singleton<ElectricalStripController> {
    private ElectricalStripData D;

    public override void OnAwake() {
        D = ElectricalStripData.Instance;
    }
}
