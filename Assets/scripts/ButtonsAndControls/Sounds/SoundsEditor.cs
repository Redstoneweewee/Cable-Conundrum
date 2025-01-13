using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundsEditor : Singleton<SoundsEditor> {
    public List<SoundsAttributes> soundEffects;
    public List<SoundsAttributes> music;

    public override IEnumerator Initialize() {
        yield return null;
    }

    void Update() {
        if(Application.isPlaying) { return; }
        if(!ScriptInitializationGlobal.Instance.ShouldUpdate) { return; }
        foreach(SoundsAttributes soundsAttribute in soundEffects) {
            if(soundsAttribute.minPitch > soundsAttribute.maxPitch) {
                soundsAttribute.minPitch = soundsAttribute.maxPitch;
            }
            if(soundsAttribute.maxPitch < soundsAttribute.minPitch) {
                soundsAttribute.maxPitch = soundsAttribute.minPitch;
            }
            if(soundsAttribute.audioSource.volume != soundsAttribute.volume) {
                soundsAttribute.audioSource.volume = soundsAttribute.volume;
            }

            if(soundsAttribute.playSound) {
                SoundPlayer.PlaySound(soundsAttribute.audioSource, new Vector2(soundsAttribute.minPitch, soundsAttribute.maxPitch), soundsAttribute.volume);
                soundsAttribute.playSound = false;
            }
        }  
    }
}
