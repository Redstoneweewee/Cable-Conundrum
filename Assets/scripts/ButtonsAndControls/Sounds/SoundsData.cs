using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsData : Singleton<SoundsData> {
    [SerializeField] public List<SoundsAttributes> soundEffects;
    [SerializeField] public List<SoundsAttributes> music;
    [Range(0, 1)] public float soundVolume = 1;
    [Range(0, 1)] public float musicVolume = 1;
    
    public override IEnumerator Initialize() {
        yield return null;
    }
}
