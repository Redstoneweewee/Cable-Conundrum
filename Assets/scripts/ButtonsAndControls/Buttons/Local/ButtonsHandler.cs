using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsHandler : ScriptInitializerBase, IPointerEnterHandler {
    [HideInInspector] public ButtonsAttributes A;

    public override IEnumerator Initialize() {
        A = Utilities.TryGetComponent<ButtonsAttributes>(gameObject);
        yield return null;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        foreach(SoundsAttributes soundsAttribute in SoundsData.Instance.soundEffects) {
            if(soundsAttribute.soundType == SoundTypes.HoverOverButton) {
                SoundPlayer.PlaySound(soundsAttribute, SoundsData.Instance.soundVolume);
            }
        }
    }

    public void OnPressButton() {
        foreach(SoundsAttributes soundsAttribute in SoundsData.Instance.soundEffects) {
            if(soundsAttribute.soundType == SoundTypes.ClickOnButton) {
                SoundPlayer.PlaySound(soundsAttribute, SoundsData.Instance.soundVolume);
            }
        }
    }
}
