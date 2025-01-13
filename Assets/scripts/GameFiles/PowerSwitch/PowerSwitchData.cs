using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSwitchData : Singleton<PowerSwitchData> {
    [SerializeField]  public GameObject                offVisual;
    [SerializeField]  public GameObject                onVisual;

    public override IEnumerator Initialize() {
        yield return null;
    }
}
