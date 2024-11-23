using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsHandler : MonoBehaviour, IPointerEnterHandler {
    [HideInInspector] public ButtonsAttributes A;

    void Awake() {
        A = Utilities.TryGetComponent<ButtonsAttributes>(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        foreach(SoundsAttributes soundsAttribute in A.soundsData.soundEffects) {
            if(soundsAttribute.soundType == SoundTypes.HoverOverButton) {
                SoundPlayer.PlaySound(soundsAttribute, A.soundsData.soundVolume);
            }
        }
    }

    public void OnPressButton() {
        foreach(SoundsAttributes soundsAttribute in A.soundsData.soundEffects) {
            if(soundsAttribute.soundType == SoundTypes.ClickOnButton) {
                SoundPlayer.PlaySound(soundsAttribute, A.soundsData.soundVolume);
            }
        }
    }
}
