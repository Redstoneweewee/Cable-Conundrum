using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SocketHandler : MonoBehaviour, IPointerDownHandler {
    SocketAttributes A;

    void Start() {
        A = GetComponent<SocketAttributes>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(!A.electricalStripData.temporarilyModifiable) { return; }
        A.isActive = !A.isActive;
        if(A.isActive) { 
            foreach(Transform child in A.childrenTransforms) { child.gameObject.SetActive(true); }
            A.electricalStripData.socketsActiveGrid[A.id.x].row[A.id.y] = true;
        }
        else { 
            foreach(Transform child in A.childrenTransforms) { child.gameObject.SetActive(false); }
            gameObject.SetActive(true);
            A.electricalStripData.socketsActiveGrid[A.id.x].row[A.id.y] = false;
        }
        A.electricalStripData.electricalStripSizeController.RenewSockets();
    }

}
