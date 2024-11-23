using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorInitializerGlobal : InitializerBase, IDataPersistence {
    List<ButtonsAttributes> enterLevelButtons = new List<ButtonsAttributes>();

    new void Awake() {
        base.Awake();
        ButtonsAttributes[] temp = FindObjectsOfType<ButtonsAttributes>();
        foreach(ButtonsAttributes buttonAttribute in temp) {
            if(buttonAttribute.buttonType == ButtonTypes.EnterLevel) {
                enterLevelButtons.Add(buttonAttribute);
            }
        }

        finishedWithAllTasks = true;
        StartCoroutine(base.SetMenuButton(true));
        StartCoroutine(base.SetLevelSelectorButton(false));
        StartCoroutine(base.SetTutorialHelpButton(false));
    }

    public IEnumerator LoadData(GameData data) {
        yield return new WaitUntil(() => finishedWithAllTasks);
        RecolorButtons(data);
    }

    public void SaveData(GameData data) {}
    public void SaveDataLate(GameData data) {}


    private void RecolorButtons(GameData data) {
        foreach(ButtonsAttributes buttonAttribute in enterLevelButtons) {
            if(buttonAttribute.levelIndex <= 0) {
                Debug.LogWarning($"An EnterLevel button has a level index of {buttonAttribute.levelIndex}. Level Indices should be 1 or greater.");
                continue;
            }
            if(data.levelCompletion[buttonAttribute.levelIndex-1]) {
                Button button = Utilities.TryGetComponentInChildren<Button>(buttonAttribute.gameObject);
                ColorBlock colors = button.colors;
                colors.normalColor      = Constants.levelSelectorFinishedButtonColor1;
                colors.highlightedColor = Constants.levelSelectorFinishedButtonColor2;
                colors.pressedColor     = Constants.levelSelectorFinishedButtonColor3;
                colors.selectedColor    = Constants.levelSelectorFinishedButtonColor1;
                button.colors = colors;
            }
        }
    }
}
