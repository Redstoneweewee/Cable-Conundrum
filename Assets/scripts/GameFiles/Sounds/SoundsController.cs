using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour {
    private SoundsData D;

    void Awake() {
        D = Utilities.TryGetComponent<SoundsData>(gameObject);
    }
    void Start() {
        InitializeSliders();
    }


    private void InitializeSliders() {
        SliderAttributes[] allSliderAttributes = FindObjectsOfType<SliderAttributes>();
        foreach(SliderAttributes sliderAttribute in allSliderAttributes) {
            switch(sliderAttribute.sliderType) {
                case SliderTypes.Sound:
                    Utilities.SubscribeToSlider(sliderAttribute.slider, OnSoundSliderChange);
                    break;
                case SliderTypes.Music:
                    Utilities.SubscribeToSlider(sliderAttribute.slider, OnMusicSliderChange);
                    break;
                default:
                    Debug.LogWarning($"Undefined slider type: {sliderAttribute.sliderType}");
                    break;
            }
        }
    }

    void OnSoundSliderChange(float newValue) {
        D.soundVolume = newValue;
    }

    void OnMusicSliderChange(float newValue) {
        D.musicVolume = newValue;
    }
}
