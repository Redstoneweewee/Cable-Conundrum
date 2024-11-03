using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablesGridAttributes {
    public bool      hasCable;
    /// <summary> Includes the indices (in the cableGeneration's Cables list) of all the cables at this grid location. </summary>
    public List<int> numbers;

    /// <summary>
    /// Initializes "hasCable" to false and "numbers" as an empty list.
    /// </summary>
    public CablesGridAttributes() {
        hasCable = false;
        numbers = new List<int>();
    }

    /// <summary>
    /// Modifies "hasCable" and adds "number" to the "numbers" list.
    /// </summary>
    public void ChangeAttributes(bool hasCable, int number) {
        this.hasCable = hasCable;
        this.numbers.Add(number);
    }
}