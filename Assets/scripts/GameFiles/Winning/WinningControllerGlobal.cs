using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningControllerGlobal : MonoBehaviour, IDataPersistence {
    private LevelInitializerGlobal levelInitializerGlobal;
    private WinningMessageSizeGlobal winningMessageSizeGlobal;
    private SoundsData soundsData;
    [SerializeField] [Range(0.01f, 1)] private float interpolationRatio = 0.05f;
    private bool hasWon = false;

    private Vector2 targetMessagePosition;


    public IEnumerator LoadData(GameData data) {
        yield return new WaitUntil(() => levelInitializerGlobal.finishedWithAllTasks);
        hasWon = data.levelCompletion[levelInitializerGlobal.levelIndex];
    }

    public void SaveData(GameData data) {}
    public void SaveDataLate(GameData data) {
        data.levelCompletion[levelInitializerGlobal.levelIndex] = hasWon;
    }


    void Awake() {
        levelInitializerGlobal   = FindFirstObjectByType<LevelInitializerGlobal>();
        winningMessageSizeGlobal = FindFirstObjectByType<WinningMessageSizeGlobal>();
        soundsData               = FindFirstObjectByType<SoundsData>();
        //used temporarily to figure out what level it is. Later should take from AllLevelsData or smth)
        int level = SceneManager.GetActiveScene().buildIndex - 1;
        string text = "Level " + level + " Complete!";
        Utilities.SetText(winningMessageSizeGlobal.text.gameObject, text);

        StationaryWinningMessage();
    }

    public void OnWin() {
        if(!hasWon) {
            foreach(SoundsAttributes soundsAttribute in soundsData.soundEffects) {
                if(soundsAttribute.soundType == SoundTypes.Victory) {
                    SoundPlayer.PlaySound(soundsAttribute, soundsData.soundVolume);
                }
            }
        }
        hasWon = true;
        DataPersistenceManager.instance.SaveGame();
    }

    void Update() {
        if(hasWon) {
            AnimateWinningMessage();
        }
        else {
            StationaryWinningMessage();
        }
    }

    private void StationaryWinningMessage() {
        winningMessageSizeGlobal.transform.position = new Vector2(Screen.width/2, Screen.height*3/2);
    }
    private void AnimateWinningMessage() {
        if(!Utilities.IsApproximate(winningMessageSizeGlobal.transform.position, targetMessagePosition, 0.001f)) {
            targetMessagePosition = new Vector2(Screen.width/2, Screen.height*0.8935f);
            winningMessageSizeGlobal.transform.position = Vector2.Lerp(winningMessageSizeGlobal.transform.position, targetMessagePosition, interpolationRatio);
        }
    }
}
