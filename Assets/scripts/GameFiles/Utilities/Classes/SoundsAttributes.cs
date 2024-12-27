using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundsAttributes {
    public AudioSource audioSource;
    public SoundTypes  soundType;
    [Min(0)] public float minPitch;
    [Min(0)] public float maxPitch;
    [Range(0, 1)] public float volume;
    public bool playSound = false;
}
