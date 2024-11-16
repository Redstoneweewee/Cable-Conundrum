using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningControllerGlobal : MonoBehaviour {
    private WinningMessageSizeGlobal winningMessageSizeGlobal;
    [SerializeField] [Range(0.01f, 1)] private float interpolationRatio = 0.05f;
    private bool hasWon = false;
    private bool isFinishedAnimating = false;

    private Vector2 targetMessagePosition;

    void Awake() {
        winningMessageSizeGlobal = FindObjectOfType<WinningMessageSizeGlobal>();
        //used temporarily to figure out what level it is. Later should take from AllLevelsData or smth)
        int level = SceneManager.GetActiveScene().buildIndex - 1;
        string text = "Level " + level + " Complete!";
        Utilities.SetText(winningMessageSizeGlobal.text.gameObject, text);

        winningMessageSizeGlobal.transform.position = new Vector2(Screen.width/2, Screen.height*1.5f);
        targetMessagePosition = new Vector2(Screen.width/2, Screen.height*0.8935f);
    }

    public void OnWin() {
        hasWon = true;
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
