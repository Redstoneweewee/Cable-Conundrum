using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderAttributes : ScriptInitializerBase {
    [HideInInspector] public Slider slider;
    [SerializeField]  public SliderTypes sliderType;

    public override IEnumerator Initialize() {
        slider = Utilities.TryGetComponentInChildren<Slider>(gameObject);
        yield return null;
    }
}
