using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptDataPersistenceBase : ScriptInitializerBase {
    public bool Loaded { get; private set; } = false;
    public bool Saved { get; private set; } = false;

    public abstract void LoadData(GameData data);

    public abstract void SaveData(GameData data);

    public IEnumerator TrackLoadCoroutine(IEnumerator coroutine) {
        Loaded = false;
        yield return coroutine;
        Loaded = true;
    }
    public IEnumerator TrackSaveCoroutine(IEnumerator coroutine) {
        Saved = false;
        yield return coroutine;
        Loaded = true;
    }
}
