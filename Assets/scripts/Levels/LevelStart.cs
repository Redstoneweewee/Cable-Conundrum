using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour {
    public bool isFinishedWithAllTasks = false;

    public void FinishedWithAllTasks() {
        isFinishedWithAllTasks = true;
        Debug.Log("set FinishedWithAllTasks to True");
    }
}
