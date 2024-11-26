using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameConfirmationGlobal : MonoBehaviour {
    [SerializeField] private GameObject exitConfirmationCanvas;




    public void OnPressEnterExitConfirmationButton() {
        Utilities.TryGetComponent<Canvas>(exitConfirmationCanvas).sortingOrder = Constants.exitConfirmationCanvasSortOrder;
    }

    public void OnPressExitExitConfirmationButton() {
        Utilities.TryGetComponent<Canvas>(exitConfirmationCanvas).sortingOrder = Constants.deactivatedCanvasSortOrder;
    }
    
    public void OnPressExitGameButton() {
        Application.Quit();
    }
}
