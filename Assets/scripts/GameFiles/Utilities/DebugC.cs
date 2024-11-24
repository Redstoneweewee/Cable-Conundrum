using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugC : MonoBehaviour {
    [SerializeField] public bool logCustomDebugMessages = true;
    [SerializeField] public bool logMathMessages = true;

    public static DebugC Get() {
        return FindObjectOfType<DebugC>();
    }

    public void Log<T>(T text) {
        if(!logCustomDebugMessages) { return; }
        Debug.Log(text);
    }
    public void LogWarning<T>(T text) {
        if(!logCustomDebugMessages) { return; }
        Debug.LogWarning(text);
    }
    public void LogArray<T>(string startText, T[] array) {
        if(!logCustomDebugMessages) { return; }
        string text = "";
        for(int i=0; i<array.Length; i++) {
            text += array[i]+", ";
        }
        Debug.Log(startText+text);
    }
    public void LogArray2D<T>(string startText, T[,] array) {
        if(!logCustomDebugMessages) { return; }
        string text = "";
        for(int i=0; i<array.GetLength(0); i++) {
            for(int j=0; j<array.GetLength(1); j++) {
                text += array[i,j]+", ";
            }
            text += "\n";
        }
        Debug.Log(startText+text);
    }
    public void LogList<T>(string startText, List<T> list) {
        if(!logCustomDebugMessages) { return; }
        string text = "";
        for(int i=0; i<list.Count; i++) {
            text += list[i]+", ";
        }
        Debug.Log(startText+text);
    }


    public void LogMath<T>(T text) {
        if(!logMathMessages) { return; }
        Debug.Log(text);
    }






    
    public void LogAlways<T>(T text) {
        Debug.Log(text);
    }
    

    public void LogArrayAlways<T>(string startText, T[] array) {
        string text = "";
        for(int i=0; i<array.Length; i++) {
            text += array[i]+", ";
        }
        Debug.Log(startText+text);
    }
    public void LogArray2DAlways<T>(string startText, T[,] array) {
        string text = "";
        for(int i=0; i<array.GetLength(0); i++) {
            for(int j=0; j<array.GetLength(1); j++) {
                text += array[i,j]+", ";
            }
            text += "\n";
        }
        Debug.Log(startText+text);
    }
    public void LogListAlways<T>(string startText, List<T> list) {
        string text = "";
        for(int i=0; i<list.Count; i++) {
            text += list[i]+", ";
        }
        Debug.Log(startText+text);
    }
}