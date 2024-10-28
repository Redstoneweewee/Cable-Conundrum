using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SocketManager : MonoBehaviour, IPointerDownHandler {
    ElectricalStripSizeController electricalStripSizeController;
    private Transform[] childrenTransforms;
    private bool isActive = true;
    public Index2D id;

    public Transform[] ChildrenTransforms {get{return childrenTransforms;} set{childrenTransforms = value;}}
    public bool IsActive {get{return isActive;} set{isActive = value;}}
    // Start is called before the first frame update
    void Start() {
        childrenTransforms = GetComponentsInChildren<Transform>();
        electricalStripSizeController = GetComponentInParent<ElectricalStripSizeController>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(!electricalStripSizeController.TemporarilyModifiable) { return; }
        isActive = !isActive;
        if(isActive) { 
            foreach(Transform child in childrenTransforms) { child.gameObject.SetActive(true); }
            electricalStripSizeController.SocketsActiveGrid[id.x].Row[id.y] = true;
        }
        else { 
            foreach(Transform child in childrenTransforms) { child.gameObject.SetActive(false); }
            gameObject.SetActive(true);
            electricalStripSizeController.SocketsActiveGrid[id.x].Row[id.y] = false;
        }
        electricalStripSizeController.RenewSockets();
    }

}
