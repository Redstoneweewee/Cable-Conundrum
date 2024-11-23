using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveSettings {
    [Range(0, 1)] public float soundVolume;
    [Range(0, 1)] public float musicVolume;

    public SaveSettings() {
        soundVolume = 1;
        musicVolume = 1;
    }
}
