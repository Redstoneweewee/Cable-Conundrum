using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SerializableData;

public class LevelBuilder {
    
    public static void BuildLevel(Level level) {
        BuildElectricalStrip(level.GetElectricalStrip());
        BuildAllObstacles(level.GetAllObstacles());
        BuildAllPlugs(level.GetAllPlugs());
    }

    public static void BuildElectricalStrip(ElectricalStrip electricalStrip) {
        if(!TestForBuildAbility()) { return; }
        GridsModifier.Instance.electricalStripSize = electricalStrip.GetElectricalStripSize();
        ElectricalStripController.Instance.ModifyBackgroundVisual();
    }
    public static void BuildAllObstacles(List<Obstacle> allPlugs) {
        if(!TestForBuildAbility()) { return; }
    }
    public static void BuildAllPlugs(List<Plug> allPlugs) {
        if(!TestForBuildAbility()) { return; }
    }

    private static bool TestForBuildAbility() {
        int issuesCount = 0;
        issuesCount += GridsModifier.Instance == null ? 1 : 0;
        issuesCount += ElectricalStripController.Instance == null ? 1 : 0;


        Debug.Log($"TestForBuildAbility issues: {issuesCount}");
        return issuesCount == 0;
    }
}
