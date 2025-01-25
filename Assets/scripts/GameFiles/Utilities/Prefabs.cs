using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Prefabs : Singleton<Prefabs> {
    public override IEnumerator Initialize() {
        yield return null;
    }
    [SerializeField] public List<GameObject> allPlugPrefabs;
    [SerializeField] public List<GameObject> AllPlugPrefabs {get {return allPlugPrefabs;} private set{}}

    
    [SerializeField] public List<GameObject> allObstaclePrefabs;
    [SerializeField] public List<GameObject> AllObstaclePrefabs {get {return allObstaclePrefabs;} private set{}}



    public GameObject FindPlugById(int id) {
        foreach(GameObject plug in allPlugPrefabs) {
            PrefabIdentifierBase prefabIdentifierBase = Utilities.TryGetComponent<PrefabIdentifierBase>(plug);
            if(prefabIdentifierBase && prefabIdentifierBase.PrefabId == id) {
                return plug;
            }
        }
        Debug.LogWarning($"Plug with id {id} cannot be found.");
        return null;
    }

    public GameObject FindObstacleById(int id) {
        foreach(GameObject obstacle in allObstaclePrefabs) {
            PrefabIdentifierBase prefabIdentifierBase = Utilities.TryGetComponent<PrefabIdentifierBase>(obstacle);
            if(prefabIdentifierBase && prefabIdentifierBase.PrefabId == id) {
                return obstacle;
            }
        }
        Debug.LogWarning($"Obstacle with id {id} cannot be found.");
        return null;
    }
}
