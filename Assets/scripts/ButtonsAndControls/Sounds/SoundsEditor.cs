using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundsEditor : MonoBehaviour {
    public List<SoundsAttributes> soundEffects;
    public List<SoundsAttributes> music;


    void Update() {
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
                Debug.Log("played sound "+soundsAttribute.audioSource.clip.name);
                SoundPlayer.PlaySound(soundsAttribute.audioSource, new Vector2(soundsAttribute.minPitch, soundsAttribute.maxPitch), soundsAttribute.volume);
                soundsAttribute.playSound = false;
            }
        }  
    }
}
