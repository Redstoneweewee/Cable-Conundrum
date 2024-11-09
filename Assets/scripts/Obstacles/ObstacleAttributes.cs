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

    [SerializeField]  public GridsSizeInitializer gridsSizeInitializer;
    [SerializeField]  public GridsController gridsController;
    [SerializeField]  public ObstacleTypes obstacleType;
    [HideInInspector] public bool          isDragging;
    [HideInInspector] public bool[,]       obstacleGrid;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Vector2       cachedMousePosition;

    void Awake() {
        obstacleHandler        = Utilities.TryGetComponent<ObstacleHandler>(gameObject);
        gridsSizeInitializer   = FindObjectOfType<GridsSizeInitializer>();
        gridsController        = FindObjectOfType<GridsController>();
        intersectionController = FindObjectOfType<IntersectionController>();
        rectTransform          = Utilities.TryGetComponentInChildren<RectTransform>(gameObject);
    }
}
