using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Obstacle : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler {
    private IntersectionController intersectionController;
    [SerializeField] private bool temporarilyModifiable;
    [SerializeField] private ObstacleTypes obstacleType;
    private bool isDragging;
    private bool[,] obstacleGrid;
    public bool          IsDragging            { get { return isDragging;            } set { isDragging            = value; } }
    public bool          TemporarilyModifiable { get { return temporarilyModifiable; } set { temporarilyModifiable = value; } }
    public ObstacleTypes ObstacleType          { get { return obstacleType;          } set { obstacleType          = value; } }
    public bool[,]       ObstacleGrid          { get { return obstacleGrid;          } set { obstacleGrid          = value; } }
    private RectTransform rectTransform;
    private Mouse   mouse = Mouse.current;
    private Vector2 cachedMousePosition;



    // Start is called before the first frame update
    void Start() {
        intersectionController = FindObjectOfType<IntersectionController>();
        rectTransform = GetComponentInChildren<RectTransform>();
        if(obstacleType == ObstacleTypes.LeftTableLeg ||
           obstacleType == ObstacleTypes.RightTableLeg ||
           obstacleType == ObstacleTypes.TableTop) { StartCoroutine(InitializeTable()); }
    }   

    private IEnumerator InitializeTable() {
        yield return new WaitForSeconds(0.01f);
        Transform [,] jointsGrid = FindObjectOfType<JointsController>().JointsGrid;
        
        if(obstacleType == ObstacleTypes.LeftTableLeg) {
            transform.position = new Vector3(jointsGrid[0, 1].position.x + Constants.jointDistance/2 + rectTransform.sizeDelta.x/2, rectTransform.sizeDelta.y/2, 0);
        }
        else if(obstacleType == ObstacleTypes.RightTableLeg) {
            transform.position = new Vector3(jointsGrid[0, jointsGrid.GetLength(1)-2].position.x - Constants.jointDistance/2 - rectTransform.sizeDelta.x/2, rectTransform.sizeDelta.y/2, 0);
        }
        
        RenewObstacleGrid();
        intersectionController.RenewAllObstaclesGrid();
        intersectionController.TestForCableIntersection();
    }
    
    
    public void OnBeginDrag(PointerEventData eventData) {
        if(!TemporarilyModifiable) { return; }
        cachedMousePosition = mouse.position.value;
    }
    public void OnDrag(PointerEventData eventData) {
        if(!TemporarilyModifiable) { return; }
        if(obstacleType != ObstacleTypes.LeftTableLeg && obstacleType != ObstacleTypes.RightTableLeg && obstacleType != ObstacleTypes.TableTop) { return; }
        //Debug.Log("Drag Begin");
        if(math.abs(cachedMousePosition.x - mouse.position.value.x) > Constants.tableSnapDistance) {
            if(mouse.position.value.x > cachedMousePosition.x) { ModifyTablePosition(Directions.Right); }
            else                                               { ModifyTablePosition(Directions.Left); }
            cachedMousePosition = mouse.position.value;
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(!TemporarilyModifiable) { return; }
        isDragging = true;
    }
    public void OnPointerUp(PointerEventData eventData) {
        if(!TemporarilyModifiable) { return; }
        isDragging = false;
    }

    private void ModifyTablePosition(Directions direction) {
        if(direction == Directions.Up || direction == Directions.Down) { Debug.LogWarning("Cannot move tables up or down as of right now."); return; }
        else if(direction == Directions.Right) {
            transform.position = new Vector3(transform.position.x + Constants.tableSnapDistance, transform.position.y, transform.position.z);
        }
        else {
            transform.position = new Vector3(transform.position.x - Constants.tableSnapDistance, transform.position.y, transform.position.z);
        }
        RenewObstacleGrid();
        intersectionController.RenewAllObstaclesGrid();
        intersectionController.TestForCableIntersection();
    }

    private void RenewObstacleGrid() {
        Transform [,] jointsGrid = FindObjectOfType<JointsController>().JointsGrid;
        obstacleGrid = new bool[jointsGrid.GetLength(0), jointsGrid.GetLength(1)];
        Vector2 topLeft     = new Vector2(transform.position.x - rectTransform.sizeDelta.x/2, transform.position.y + rectTransform.sizeDelta.y/2);
        Vector2 bottomRight = new Vector2(transform.position.x + rectTransform.sizeDelta.x/2, transform.position.y - rectTransform.sizeDelta.y/2);

        Index2D startingIndex = new Index2D(0, (int)((topLeft.x - jointsGrid[0,0].position.x)/Constants.jointDistance)+1);
        Index2D endingIndex = new Index2D(obstacleGrid.GetLength(0)-1, (int)((bottomRight.x-0.1f - jointsGrid[0,0].position.x)/Constants.jointDistance));
        startingIndex = Utilities.ClampIndex2D(startingIndex, 0, jointsGrid.GetLength(0), 0, jointsGrid.GetLength(1));
        endingIndex   = Utilities.ClampIndex2D(endingIndex, 0, jointsGrid.GetLength(0), 0, jointsGrid.GetLength(1));
        Debug.Log($"startingIndex: ({startingIndex.x}, {startingIndex.y})");
        Debug.Log($"endingIndex: ({endingIndex.x}, {endingIndex.y})");

        for(int i=startingIndex.x; i<=endingIndex.x; i++) {
            for(int j=startingIndex.y; j<=endingIndex.y; j++) {
                obstacleGrid[i, j] = true;
            }
        }
        intersectionController.RenewAllObstaclesGrid();
    }
    
    public void SetOpacity(float opacity) {
        CanvasGroup canvasGroup = GetComponentInChildren<CanvasGroup>();
        canvasGroup.alpha = opacity;
        //Image[] cableImages = GetComponentsInChildren<Image>();
        //foreach(Image cableImage in cableImages) {
        //    cableImage.color = new Color(cableImage.color.r,
        //                                 cableImage.color.g,
        //                                 cableImage.color.b,
        //                                 opacity);
        //}
        if(obstacleType == ObstacleTypes.Plug) {
            Image plugImage = GetComponent<PlugInteractions>().PlugVisual.GetComponent<Image>();
            plugImage.color = new Color(plugImage.color.r,
                                        plugImage.color.g,
                                        plugImage.color.b,
                                        1.0f);
        }
    }
}
