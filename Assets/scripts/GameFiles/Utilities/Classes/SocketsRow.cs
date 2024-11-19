using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SocketsRow {
    [SerializeField] public List<bool> row = new List<bool>();

    /// <summary>
    /// Adds length amount of true to "row".
    /// </summary>
    public SocketsRow(int length) {
        for(int i=0; i<length; i++) { row.Add(true); }
    }

    /// <summary>
    /// Initializes the first elements to previousSocketsRow's values (depending on the length of previousSocketsRow).
    /// The rest up to "max" are set to true.
    /// </summary>
    public SocketsRow(SocketsRow previousSocketsRow, int max) {
        for(int i=0; i<max; i++) {
            if(i < previousSocketsRow.row.Count) { row.Add(previousSocketsRow.row[i]); }
            else { row.Add(true); }
        }
    }
}