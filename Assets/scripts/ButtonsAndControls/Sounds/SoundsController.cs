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
        InitializeSounds();
    }


    private void InitializeSliders() {
        SliderAttributes[] allSliderAttributes = FindObjectsOfType<SliderAttributes>();
        foreach(SliderAttributes sliderAttribute in allSliderAttributes) {
            sliderAttribute.slider.onValueChanged.RemoveAllListeners();
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

    private void InitializeSounds() {
        D.soundEffects = D.soundsEditor.soundEffects;
        D.music = D.soundsEditor.music;
    }

    void OnSoundSliderChange(float newValue) {
        D.soundVolume = newValue;
    }

    void OnMusicSliderChange(float newValue) {
        D.musicVolume = newValue;
    }
}
