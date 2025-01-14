using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsData : Singleton<SoundsData> {
    [SerializeField] public List<SoundsAttributes> soundEffects;
    [SerializeField] public List<SoundsAttributes> music;
    [Range(0, 1)] public float soundVolume = 0.75f;
    [Range(0, 1)] public float musicVolume = 0.75f;
    
    public override IEnumerator Initialize() {
        yield return null;
    }
}
