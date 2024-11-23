using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundsEditor : MonoBehaviour {
    public List<SoundsAttributes> soundEffects;
    public List<SoundsAttributes> music;


    void Update() {
        if(Application.isPlaying) { return; }
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
