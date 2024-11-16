using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActivity {
    public GameObject gameObject;
    public bool isInitiallyActive;

    public GameObjectActivity(GameObject gameObject, bool isInitiallyActive) {
        this.gameObject = gameObject;
        this.isInitiallyActive = isInitiallyActive;
    }
}
