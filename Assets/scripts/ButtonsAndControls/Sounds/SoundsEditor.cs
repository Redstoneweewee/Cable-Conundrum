using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundsEditor : MonoBehaviour {
    public List<SoundsAttributes> soundEffects;
    public List<SoundsAttributes> music;


    void Update() {
        foreach(SoundsAttributes soundAttribute in soundEffects) {
            if(soundAttribute.minPitch > soundAttribute.maxPitch) {
                soundAttribute.minPitch = soundAttribute.maxPitch;
            }
            if(soundAttribute.maxPitch < soundAttribute.minPitch) {
                soundAttribute.maxPitch = soundAttribute.minPitch;
            }
            if(soundAttribute.audioSource.volume != soundAttribute.volume) {
                soundAttribute.audioSource.volume = soundAttribute.volume;
            }

            if(soundAttribute.playSound) {
                SoundPlayer.PlaySound(soundAttribute.audioSource, new Vector2(soundAttribute.minPitch, soundAttribute.maxPitch), soundAttribute.volume);
                soundAttribute.playSound = false;
            }
        }  
    }
}
