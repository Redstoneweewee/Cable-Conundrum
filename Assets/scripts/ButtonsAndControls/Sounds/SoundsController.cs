using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : Singleton<SoundsController> {
    private SoundsData D;

    public override IEnumerator Initialize() {
        D = SoundsData.Instance;
        InitializeSounds();
        yield return null;
    }


    public void InitializeSliders() {
        SliderAttributes[] allSliderAttributes = FindObjectsByType<SliderAttributes>(FindObjectsSortMode.None);
        foreach(SliderAttributes sliderAttribute in allSliderAttributes) {
            sliderAttribute.slider.onValueChanged.RemoveAllListeners();
            switch(sliderAttribute.sliderType) {
                case SliderTypes.Sound:
                    sliderAttribute.slider.value = D.soundVolume;
                    Utilities.SubscribeToSlider(sliderAttribute.slider, OnSoundSliderChange);
                    break;
                case SliderTypes.Music:
                    sliderAttribute.slider.value = D.musicVolume;
                    Utilities.SubscribeToSlider(sliderAttribute.slider, OnMusicSliderChange);
                    break;
                default:
                    Debug.LogError($"Undefined slider type: {sliderAttribute.sliderType}");
                    break;
            }
        }
    }

    private void InitializeSounds() {
        D.soundEffects = SoundsEditor.Instance.soundEffects;
        D.music = SoundsEditor.Instance.music;
    }

    void OnSoundSliderChange(float newValue) {
        D.soundVolume = newValue;
    }

    void OnMusicSliderChange(float newValue) {
        D.musicVolume = newValue;
    }
}
