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
    private bool isFinishedAnimating = false;

    private Vector2 targetMessagePosition;


    public IEnumerator LoadData(GameData data) {
        yield return null;
        hasWon = data.levelCompletion[levelInitializerGlobal.levelIndex];
    }

    public void SaveData(GameData data) {
        data.levelCompletion[levelInitializerGlobal.levelIndex] = hasWon;
    }


    void Awake() {
        levelInitializerGlobal   = FindObjectOfType<LevelInitializerGlobal>();
        winningMessageSizeGlobal = FindObjectOfType<WinningMessageSizeGlobal>();
        soundsData               = FindObjectOfType<SoundsData>();
        //used temporarily to figure out what level it is. Later should take from AllLevelsData or smth)
        int level = SceneManager.GetActiveScene().buildIndex - 1;
        string text = "Level " + level + " Complete!";
        Utilities.SetText(winningMessageSizeGlobal.text.gameObject, text);

        winningMessageSizeGlobal.transform.position = new Vector2(Screen.width/2, Screen.height*1.5f);
        targetMessagePosition = new Vector2(Screen.width/2, Screen.height*0.8935f);
    }

    public void OnWin() {
        if(!hasWon) {
            foreach(SoundsAttributes soundsAttribute in soundsData.soundEffects) {
                if(soundsAttribute.soundType == SoundTypes.Victory) {
                    SoundPlayer.PlaySound(soundsAttribute);
                }
            }
        }
        hasWon = true;
        DataPersistenceManager.instance.SaveGame();
    }

    void Update() {
        if(hasWon && !isFinishedAnimating) {
            AnimateWinningMessage();
        }
    }

    private void AnimateWinningMessage() {
        if(!Utilities.IsApproximate(winningMessageSizeGlobal.transform.position, targetMessagePosition, 0.001f)) {
            winningMessageSizeGlobal.transform.position = Vector2.Lerp(winningMessageSizeGlobal.transform.position, targetMessagePosition, interpolationRatio);
        }
    }
}
