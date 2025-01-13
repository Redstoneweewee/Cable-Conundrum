using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SocketHandler : ScriptInitializerBase, IPointerDownHandler {
    SocketAttributes A;

    public override IEnumerator Initialize() {
        A = Utilities.TryGetComponent<SocketAttributes>(gameObject);
        yield return null;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(!ElectricalStripData.Instance.temporarilyModifiable) { return; }
        A.isActive = !A.isActive;
        if(A.isActive) { 
            foreach(Transform child in A.childrenTransforms) { child.gameObject.SetActive(true); }
            GridsData.Instance.socketsActiveGrid[A.id.x].row[A.id.y] = true;
        }
        else { 
            foreach(Transform child in A.childrenTransforms) { child.gameObject.SetActive(false); }
            gameObject.SetActive(true);
            GridsData.Instance.socketsActiveGrid[A.id.x].row[A.id.y] = false;
        }
        //A.electricalStripData.electricalStripSizeController.RenewSockets();
        GridsController.Instance.RenewSocketsGrid();
    }

}
