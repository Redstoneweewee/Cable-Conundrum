using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SerializableData;

public class LevelBuilder {
    

    public static void BuildLevel(Level level) {
        BuildElectricalStrip(level.GetElectricalStrip());
        BuildAllObstacles(level.GetAllPlugObstacles());
        BuildAllPlugs(level.GetAllPlugs());
    }


    public static void BuildElectricalStrip(ElectricalStrip electricalStrip) {
        if(!TestForBuildability()) { return; }
        GridsModifier.Instance.electricalStripSize = electricalStrip.GetElectricalStripSize();
        ElectricalStripController.Instance.ModifyBackgroundVisual();
    }


    public static void BuildAllObstacles(List<PlugObstacle> allPlugObstacles) {
        if(!TestForBuildability()) { return; }
        foreach(PlugObstacle plugObstacleScript in allPlugObstacles) {
            GameObject obstaclePrefab = Prefabs.Instance.FindObstacleById(plugObstacleScript.GetObstacleId());
            
            Transform obstaclesCanvas = FindCanvas(CanvasType.ObstaclesCanvas).transform;

            GameObject plugObstacle = GameObject.Instantiate(obstaclePrefab, obstaclesCanvas);
            
            //Move obstacle to socketsIndex
            plugObstacle.transform.position = Utilities.CalculatePlugPosition(plugObstacleScript.GetSocketsIndex(), Utilities.TryGetComponent<PlugAttributes>(plugObstacle).localSnapPositions[0]);
            
            //TODO - Generated cables from cableIndexAndDirections
        }
    }


    public static void BuildAllPlugs(List<Plug> allPlugs) {
        if(!TestForBuildability()) { return; }
    }

    private static bool TestForBuildability() {
        //Shouldn't be necessary but will be here just in case
        int issuesCount = 0;
        issuesCount += GridsModifier.Instance == null ? 1 : 0;
        issuesCount += ElectricalStripController.Instance == null ? 1 : 0;
        issuesCount += Prefabs.Instance == null ? 1 : 0;


        Debug.Log($"TestForBuildAbility issues: {issuesCount}");
        return issuesCount == 0;
    }

    private static GameObject FindCanvas(CanvasType canvasType) {
        foreach(CanvasEnums canvas in GameObject.FindObjectsByType<CanvasEnums>(FindObjectsSortMode.None)) {
            if(canvas.CanvasType == canvasType) {
                return canvas.gameObject;
            }
        }
        Debug.LogWarning($"Cannot find a canvas with canvas type {canvasType} in the current scene.");
        return null;
    }
}
