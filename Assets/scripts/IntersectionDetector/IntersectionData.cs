using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntersectionData : MonoBehaviour {
    [HideInInspector] public bool hasIntersection = false;
    [HideInInspector] public ElectricalStripData electricalStripData;
    [HideInInspector] public JointsData jointsData;
    [HideInInspector] public bool[,] allObstaclesGrid;

    // Start is called before the first frame update
    void Awake() {
        electricalStripData = FindObjectOfType<ElectricalStripData>();
        jointsData = FindObjectOfType<JointsData>();
    }
}
