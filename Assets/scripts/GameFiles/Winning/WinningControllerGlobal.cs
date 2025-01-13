using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningControllerGlobal : Singleton<WinningControllerGlobal>, IDataPersistence {
    [SerializeField] [Range(0.01f, 1)] private float interpolationRatio = 0.05f;
    private bool hasWon = false;

    private Vector2 targetMessagePosition;


    public IEnumerator LoadData(GameData data) { 
        yield return null;
    }

    public void SaveData(GameData data) {}
    public void SaveDataLate(GameData data) {
        data.levelCompletion[LevelInitializerGlobal.Instance.levelIndex] = hasWon;
    }

    public override IEnumerator Initialize() {
        //used temporarily to figure out what level it is. Later should take from AllLevelsData or smth)
        int level = LevelInitializerGlobal.Instance.levelIndex + 1;
        string text = "Level " + level + " Complete!";
        Utilities.SetText(WinningMessageSizeGlobal.Instance.text.gameObject, text);

        StationaryWinningMessage();
        yield return null;
    }

    public void OnWin() {
        Debug.Log("OnWIn");
        if(!hasWon) {
            foreach(SoundsAttributes soundsAttribute in SoundsData.Instance.soundEffects) {
                if(soundsAttribute.soundType == SoundTypes.Victory) {
                    SoundPlayer.PlaySound(soundsAttribute, SoundsData.Instance.soundVolume);
                }
            }
        }
        hasWon = true;
        DataPersistenceManager.Instance.SaveGame();
    }

    void Update() {
        if(!ScriptInitializationGlobal.Instance.ShouldUpdate) { return; }
        if(hasWon) {
            AnimateWinningMessage();
        }
        else {
            StationaryWinningMessage();
        }
    }

    private void StationaryWinningMessage() {
        WinningMessageSizeGlobal.Instance.transform.position = new Vector2(Screen.width/2, Screen.height*3/2);
    }
    private void AnimateWinningMessage() {
        if(!Utilities.IsApproximate(WinningMessageSizeGlobal.Instance.transform.position, targetMessagePosition, 0.001f)) {
            targetMessagePosition = new Vector2(Screen.width/2, Screen.height*0.8935f);
            WinningMessageSizeGlobal.Instance.transform.position = Vector2.Lerp(WinningMessageSizeGlobal.Instance.transform.position, targetMessagePosition, interpolationRatio);
        }
    }
}
