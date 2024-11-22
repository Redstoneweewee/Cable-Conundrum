using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundPlayer : MonoBehaviour {
    public static void PlaySound(AudioSource audioSource, Vector2 pitchRange, float volume) {
        float pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
    }
}
