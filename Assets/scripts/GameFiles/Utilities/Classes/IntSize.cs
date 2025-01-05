using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct IntSize {
    [Min(0)] public int width;
    [Min(0)] public int height;
    public IntSize(int width, int height) {
        this.width = width;
        this.height = height;
    }

    // Overloads the == operator
    public static bool operator ==(IntSize a, IntSize b) { return a.width == b.width && a.height == b.height; }

    // Overloads the != operator
    public static bool operator !=(IntSize a, IntSize b) { return !(a == b); }
    
    // It's still a good practice to override Equals and GetHashCode
    public override bool Equals(object obj) {
        if (!(obj is IntSize))
            return false;

        IntSize other = (IntSize)obj;
        return this == other;
    }
    public override int GetHashCode() { return (width, height).GetHashCode(); }
}