using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundPlayer {
    public static void PlaySound(AudioSource audioSource, Vector2 pitchRange, float volume) {
        float pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
    }
    
    public static void PlaySound(SoundsAttributes soundsAttribute, float volume) {
        float pitch = Random.Range(soundsAttribute.minPitch, soundsAttribute.maxPitch);
        soundsAttribute.audioSource.pitch = pitch;
        soundsAttribute.audioSource.volume = soundsAttribute.volume * volume;
        soundsAttribute.audioSource.Play();
    }
}
