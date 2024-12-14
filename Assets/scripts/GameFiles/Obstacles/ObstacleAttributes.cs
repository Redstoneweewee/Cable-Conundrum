using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ObstacleAttributes : MonoBehaviour {
    [HideInInspector] public ObstacleHandler        obstacleHandler;
    [HideInInspector] public Mouse                  mouse = Mouse.current;
    [HideInInspector] public IntersectionController intersectionController;
    [SerializeField]  public bool                   temporarilyModifiable;

    [SerializeField]  public GridsSkeleton gridsSkeleton;
    [SerializeField]  public GridsController gridsController;
    [SerializeField]  public ObstacleTypes obstacleType;
    [HideInInspector] public bool          isDragging;
    [HideInInspector] public bool[,]       obstacleGrid;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Vector2       cachedMousePosition;
    [HideInInspector] public float         cachedLeftMostX;
    [HideInInspector] public float         cachedRightMostX;

    void Awake() {
        obstacleHandler        = Utilities.TryGetComponent<ObstacleHandler>(gameObject);
        gridsSkeleton          = FindFirstObjectByType<GridsSkeleton>();
        gridsController        = FindFirstObjectByType<GridsController>();
        intersectionController = FindFirstObjectByType<IntersectionController>();
        rectTransform          = Utilities.TryGetComponentInChildren<RectTransform>(gameObject);
    }
}
