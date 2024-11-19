using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderAttributes : MonoBehaviour {
    [HideInInspector] public Slider slider;
    [SerializeField]  public SliderTypes sliderType;

    void Awake() {
        slider = Utilities.TryGetComponentInChildren<Slider>(gameObject);
    }
}
