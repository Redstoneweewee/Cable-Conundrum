using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Index2D {
    public int x;
    public int y;
    public Index2D(int x, int y) {
        this.x = x;
        this.y = y;
    }

    // Overloads the == operator
    public static bool operator ==(Index2D a, Index2D b) { return a.x == b.x && a.y == b.y; }

    // Overloads the != operator
    public static bool operator !=(Index2D a, Index2D b) { return !(a == b); }
    
    // It's still a good practice to override Equals and GetHashCode
    public override bool Equals(object obj) {
        if (!(obj is Index2D))
            return false;

        Index2D other = (Index2D)obj;
        return this == other;
    }
    public override int GetHashCode() { return (x, y).GetHashCode(); }
}