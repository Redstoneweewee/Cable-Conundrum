using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ObstacleAttributes : PrefabIdentifierBase {
    [HideInInspector] public ObstacleHandler        obstacleHandler;
    [SerializeField]  public bool                   temporarilyModifiable;

    [SerializeField]  public ObstacleTypes   obstacleType;
    [HideInInspector] public bool            isDragging;
    [HideInInspector] public bool[,]         obstacleGrid;
    [HideInInspector] public RectTransform   rectTransform;
    [HideInInspector] public Vector2         cachedMousePosition;
    [HideInInspector] public float           cachedLeftMostX;
    [HideInInspector] public float           cachedRightMostX;

    public override IEnumerator Initialize() {
        obstacleHandler = Utilities.TryGetComponent<ObstacleHandler>(gameObject);
        rectTransform   = Utilities.TryGetComponentInChildren<RectTransform>(gameObject);
        yield return null;
    }
}
