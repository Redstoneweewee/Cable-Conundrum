using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameConfirmationGlobal : Singleton<ExitGameConfirmationGlobal> {
    [SerializeField] private GameObject exitConfirmationCanvas;

    public override void OnAwake() { }


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
