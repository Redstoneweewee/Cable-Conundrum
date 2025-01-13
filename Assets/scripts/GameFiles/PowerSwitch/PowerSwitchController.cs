using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerSwitchController : Singleton<PowerSwitchController>, IPointerClickHandler, IDataPersistence {
    private PowerSwitchData D;

    public IEnumerator LoadData(GameData data) {
        yield return new WaitUntil(() => LevelInitializerGlobal.Instance.finishedWithAllTasks);
        if(data.levelCompletion[LevelInitializerGlobal.Instance.levelIndex]) {
            Win();
        }
    }

    public void SaveData(GameData data) {}
    public void SaveDataLate(GameData data) {}


    public override IEnumerator Initialize() {
        D = Utilities.TryGetComponent<PowerSwitchData>(gameObject);
        yield return null;
    }

    public void OnPointerClick(PointerEventData eventData) {
        LevelFailureTypes levelFailureTypes = TestForLevelSuccess();
        if     (levelFailureTypes == LevelFailureTypes.Plugs)     { Debug.Log("Not all plugs are plugged in!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Obstacles) { Debug.Log("Some cables are in obstacles!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.Cables)    { Debug.Log("Some cables are overlapping!"); DidNotWin(); }
        else if(levelFailureTypes == LevelFailureTypes.None)      { Debug.Log("You win!"); Win(); }
        else   { Debug.LogError("Undefined level failure type."); }
        DebugC.Instance?.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
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

        if(IntersectionData.Instance.hasIntersection) {
            return LevelFailureTypes.Cables;
        }
        return LevelFailureTypes.None;
    }

    private void Win() {
        D.offVisual.SetActive(false);
        D.onVisual.SetActive(true);
        WinningControllerGlobal.Instance.OnWin();
    }

    private void DidNotWin() {
        D.offVisual.SetActive(true);
        D.onVisual.SetActive(false);
    }
}
