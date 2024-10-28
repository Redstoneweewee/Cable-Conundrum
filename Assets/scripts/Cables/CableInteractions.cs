using UnityEngine;
using UnityEngine.EventSystems;

public class CableInteractions : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Triggered");
    }
}