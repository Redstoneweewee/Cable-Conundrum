using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ObstacleHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler {
    private ObstacleAttributes A;

    void Awake() {
        A = Utilities.TryGetComponent<ObstacleAttributes>(gameObject);
    }

    void Start() {
        if(A.obstacleType == ObstacleTypes.LeftTableLeg ||
           A.obstacleType == ObstacleTypes.RightTableLeg ||
           A.obstacleType == ObstacleTypes.TableTop) { StartCoroutine(InitializeTable()); }
        else if(A.obstacleType == ObstacleTypes.Screw) {
            StartCoroutine(InitializeScrew());
        }
    }   

    void Update() {
        if(A.obstacleType != ObstacleTypes.TableTop) { return; }
        RenewTableTopPosition();
    }

    
    private void RenewTableTopPosition() {
        ObstacleAttributes[] obstacleAttributes = FindObjectsByType<ObstacleAttributes>(FindObjectsSortMode.None);
        ObstacleAttributes leftLegObstacle = null;
        ObstacleAttributes rightLegObstacle = null;
        foreach(ObstacleAttributes obstacleAttribute in obstacleAttributes) {
            if(obstacleAttribute.obstacleType == ObstacleTypes.LeftTableLeg) {
                if(leftLegObstacle == null || obstacleAttribute.transform.position.x > leftLegObstacle.transform.position.x) {
                    leftLegObstacle = obstacleAttribute;
                }
            }
            else if(obstacleAttribute.obstacleType == ObstacleTypes.RightTableLeg) {
                if(rightLegObstacle == null || obstacleAttribute.transform.position.x < rightLegObstacle.transform.position.x) {
                    rightLegObstacle = obstacleAttribute;
                }
            }
        }
        float leftMostX = 0;
        float rightMostX = 1920;
        if(leftLegObstacle) {
            leftMostX = leftLegObstacle.transform.position.x + leftLegObstacle.GetComponentInChildren<RectTransform>().sizeDelta.x/2 - LevelResizeGlobal.instance.tableTopDistanceFromLeg;
        }
        if(rightLegObstacle) {
            rightMostX = rightLegObstacle.transform.position.x - rightLegObstacle.GetComponentInChildren<RectTransform>().sizeDelta.x/2 + LevelResizeGlobal.instance.tableTopDistanceFromLeg;
        }
        A.rectTransform.sizeDelta = new Vector2(rightMostX-leftMostX, A.rectTransform.sizeDelta.y);
        transform.position = new Vector3(leftMostX+A.rectTransform.sizeDelta.x/2, transform.position.y, 0);
        transform.SetSiblingIndex(transform.parent.childCount-1);

        if(A.cachedLeftMostX != leftMostX || A.cachedRightMostX != rightMostX) {
            RenewObstacleGrid();
            A.gridsController.RenewAllObstaclesGrid();
            A.intersectionController.TestForCableIntersection();
        }
        A.cachedLeftMostX = leftMostX;
        A.cachedRightMostX = rightMostX;
    }

    private IEnumerator InitializeTable() {
        yield return new WaitUntil(() => A.gridsData.initialized);
        Vector2[,] skeletonGrid = A.gridsSkeleton.jointsSkeletonGrid;
        
        if(A.obstacleType == ObstacleTypes.LeftTableLeg) {
            transform.position = new Vector3(skeletonGrid[0, 1].x + LevelResizeGlobal.instance.jointDistance/2 + A.rectTransform.sizeDelta.x/2, A.rectTransform.sizeDelta.y/2, 0);
        }
        else if(A.obstacleType == ObstacleTypes.RightTableLeg) {
            transform.position = new Vector3(skeletonGrid[0, skeletonGrid.GetLength(1)-2].x - LevelResizeGlobal.instance.jointDistance/2 - A.rectTransform.sizeDelta.x/2, A.rectTransform.sizeDelta.y/2, 0);
        }
        
        RenewObstacleGrid();
        A.gridsController.RenewAllObstaclesGrid();
        A.intersectionController.TestForCableIntersection();
    }
    private IEnumerator InitializeScrew() {
        yield return new WaitUntil(() => A.gridsData.initialized);
        RenewObstacleGrid();
        A.gridsController.RenewAllObstaclesGrid();
        A.intersectionController.TestForCableIntersection();
    }
    
    
    public void OnBeginDrag(PointerEventData eventData) {
        if(!A.temporarilyModifiable) { return; }
        A.cachedMousePosition = A.mouse.position.value;
    }
    public void OnDrag(PointerEventData eventData) {
        if(!A.temporarilyModifiable) { return; }
        if(A.obstacleType == ObstacleTypes.LeftTableLeg || A.obstacleType == ObstacleTypes.RightTableLeg || A.obstacleType == ObstacleTypes.TableTop) { 
            if(math.abs(A.cachedMousePosition.x - A.mouse.position.value.x) > LevelResizeGlobal.instance.tableSnapDistance) {
                if(A.mouse.position.value.x > A.cachedMousePosition.x) { ModifyTablePosition(Directions.Right); }
                else                                               { ModifyTablePosition(Directions.Left); }
                A.cachedMousePosition = A.mouse.position.value;
            }
        }
        else if(A.obstacleType == ObstacleTypes.Screw) {
            Vector2 deltaPosition = new Vector2(math.abs(transform.position.x - A.mouse.position.value.x), math.abs(transform.position.y - A.mouse.position.value.y));
            if(deltaPosition.x > LevelResizeGlobal.instance.jointDistance) {
                int repeat = (int)(deltaPosition.x/LevelResizeGlobal.instance.jointDistance);
                for(int i=0; i<1; i++) {
                    if     (A.mouse.position.value.x < transform.position.x) {
                        ModifyScrewPosition(Directions.Left);
                        //A.cachedMousePosition = new Vector2(A.cachedMousePosition.x-LevelResizeGlobal.instance.jointDistance, A.cachedMousePosition.y);
                    }
                    else if(A.mouse.position.value.x > transform.position.x) {
                        ModifyScrewPosition(Directions.Right);
                        //A.cachedMousePosition = new Vector2(A.cachedMousePosition.x+LevelResizeGlobal.instance.jointDistance, A.cachedMousePosition.y);
                    }
                }
            }
            else if(deltaPosition.y > LevelResizeGlobal.instance.jointDistance) {
                int repeat = (int)(deltaPosition.y/LevelResizeGlobal.instance.jointDistance);
                for(int i=0; i<1; i++) {
                    if     (A.mouse.position.value.y > transform.position.y) { 
                        ModifyScrewPosition(Directions.Up);
                        //A.cachedMousePosition = new Vector2(A.cachedMousePosition.x, A.cachedMousePosition.y+LevelResizeGlobal.instance.jointDistance);
                    }
                    else if(A.mouse.position.value.y < transform.position.y) {
                        ModifyScrewPosition(Directions.Down);
                        //A.cachedMousePosition = new Vector2(A.cachedMousePosition.x, A.cachedMousePosition.y-LevelResizeGlobal.instance.jointDistance);
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(!A.temporarilyModifiable) { return; }
        A.isDragging = true;
    }
    public void OnPointerUp(PointerEventData eventData) {
        if(!A.temporarilyModifiable) { return; }
        A.isDragging = false;
    }

    private void ModifyTablePosition(Directions direction) {
        if(direction == Directions.Up || direction == Directions.Down) { Debug.LogWarning("Cannot move tables up or down as of right now."); return; }
        else if(direction == Directions.Right) {
            transform.position = new Vector3(transform.position.x + LevelResizeGlobal.instance.tableSnapDistance, transform.position.y, transform.position.z);
        }
        else {
            transform.position = new Vector3(transform.position.x - LevelResizeGlobal.instance.tableSnapDistance, transform.position.y, transform.position.z);
        }
        RenewObstacleGrid();
        A.gridsController.RenewAllObstaclesGrid();
        A.intersectionController.TestForCableIntersection();
    }

    private void ModifyScrewPosition(Directions direction) {
        Vector3 newPosition;
        if(direction == Directions.Up) {
            newPosition = new Vector3(transform.position.x, transform.position.y + LevelResizeGlobal.instance.jointDistance, transform.position.z);
        }
        else if(direction == Directions.Down) {
            newPosition = new Vector3(transform.position.x, transform.position.y - LevelResizeGlobal.instance.jointDistance, transform.position.z);
        }
        else if(direction == Directions.Right) {
            newPosition = new Vector3(transform.position.x + LevelResizeGlobal.instance.jointDistance, transform.position.y, transform.position.z);
        }
        else {
            newPosition = new Vector3(transform.position.x - LevelResizeGlobal.instance.jointDistance, transform.position.y, transform.position.z);
        }
        if(newPosition.x < 0 || newPosition.x > Screen.width || newPosition.y < 0 || newPosition.y > Screen.height) {
            return;
        }
        transform.position = newPosition;
        RenewObstacleGrid();
        A.gridsController.RenewAllObstaclesGrid();
        A.intersectionController.TestForCableIntersection();
    }

    private void RenewObstacleGrid() {
        //Plug obstacles do not use this
        Vector2[,] skeletonGrid = A.gridsSkeleton.jointsSkeletonGrid;
        A.obstacleGrid = new bool[skeletonGrid.GetLength(0), skeletonGrid.GetLength(1)];

        if(A.obstacleType == ObstacleTypes.LeftTableLeg || A.obstacleType == ObstacleTypes.RightTableLeg || A.obstacleType == ObstacleTypes.TableTop) {
            Vector2 topLeft     = new Vector2(transform.position.x - A.rectTransform.sizeDelta.x/2, transform.position.y + A.rectTransform.sizeDelta.y/2);
            Vector2 bottomRight = new Vector2(transform.position.x + A.rectTransform.sizeDelta.x/2, transform.position.y - A.rectTransform.sizeDelta.y/2);
            
            Index2D startingIndex = new Index2D(0, (int)((topLeft.x - skeletonGrid[0,0].x)/LevelResizeGlobal.instance.jointDistance)+1);
            Index2D endingIndex = new Index2D(A.obstacleGrid.GetLength(0)-1, (int)((bottomRight.x-0.1f - skeletonGrid[0,0].x)/LevelResizeGlobal.instance.jointDistance));
            startingIndex = Utilities.ClampIndex2D(startingIndex, 0, skeletonGrid.GetLength(0), 0, skeletonGrid.GetLength(1));
            endingIndex   = Utilities.ClampIndex2D(endingIndex, 0, skeletonGrid.GetLength(0), 0, skeletonGrid.GetLength(1));
            DebugC.Get()?.Log($"startingIndex: ({startingIndex.x}, {startingIndex.y})");
            DebugC.Get()?.Log($"endingIndex: ({endingIndex.x}, {endingIndex.y})");

            for(int i=startingIndex.x; i<=endingIndex.x; i++) {
                for(int j=startingIndex.y; j<=endingIndex.y; j++) {
                    A.obstacleGrid[i, j] = true;
                }
            } 
        }
        else if(A.obstacleType == ObstacleTypes.Screw) {
            Index2D obstacleIndex = Utilities.CalculateJointsGridIndex(transform.position);
            A.obstacleGrid[obstacleIndex.x, obstacleIndex.y] = true;
        }
    }
    
    public void SetOpacity(float opacity) {
        CanvasGroup canvasGroup = Utilities.TryGetComponentInChildren<CanvasGroup>(gameObject);
        canvasGroup.alpha = opacity;
        if(A.obstacleType == ObstacleTypes.Plug) {
            Image plugImage = Utilities.TryGetComponent<Image>(Utilities.TryGetComponent<PlugAttributes>(gameObject).plugVisual);
            plugImage.color = new Color(plugImage.color.r,
                                        plugImage.color.g,
                                        plugImage.color.b,
                                        1.0f);
        }
    }
}
