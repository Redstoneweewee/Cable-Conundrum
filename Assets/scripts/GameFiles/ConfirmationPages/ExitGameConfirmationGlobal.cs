using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameConfirmationGlobal : MonoBehaviour {
    [SerializeField] private GameObject exitConfirmationCanvas;




    public void OnPressEnterExitConfirmationButton() {
        exitConfirmationCanvas.SetActive(true);
    }

    public void OnPressExitExitConfirmationButton() {
        exitConfirmationCanvas.SetActive(false);
    }
    
    public void OnPressExitGameButton() {
        Application.Quit();
    }
}
