using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSwitchController : MonoBehaviour, IPointerClickHandler, IDataPersistence {
    private PowerSwitchData D;

    public IEnumerator LoadData(GameData data) {
        yield return null;
        if(data.levelCompletion[D.levelInitializerGlobal.levelIndex]) {
            Win();
        }
    }

    public void SaveData(GameData data) {}
    public void SaveDataLate(GameData data) {}


    void Awake() {
        D = Utilities.TryGetComponent<PowerSwitchData>(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        LevelFailureTypes levelFailureTypes = TestForLevelSuccess();
        if     (levelFailureTypes == LevelFailureTypes.Plugs)     { Debug.Log("Not all plugs are plugged in!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Obstacles) { Debug.Log("Some cables are in obstacles!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Cables)    { Debug.Log("Some cables are overlapping!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.None)      { Debug.Log("You win!"); Win(); }
        else   { Debug.LogError("Undefined level failure type."); }
        DebugC.Get()?.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    //conditions:
    //all plugs must be plugged in
    //no cables are overlapping
    private LevelFailureTypes TestForLevelSuccess() {
        PlugAttributes[] allPlugAttributes = FindObjectsByType<PlugAttributes>(FindObjectsSortMode.None);
        foreach(PlugAttributes plugAttributes in allPlugAttributes) {
            if(plugAttributes.isObstacle) { continue; }
            if(!plugAttributes.isPluggedIn) { return LevelFailureTypes.Plugs; }
        }

        if(D.intersectionData.hasIntersection) {
            return LevelFailureTypes.Cables;
        }
        return LevelFailureTypes.None;
    }

    private void Win() {
        D.offVisual.SetActive(false);
        D.onVisual.SetActive(true);
        D.winningControllerGlobal.OnWin();
    }

    private void DidNotWin() {
        D.offVisual.SetActive(true);
        D.onVisual.SetActive(false);
    }
}
