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
    
    //public static implicit operator Index2DFromCenter(Index2D a) {
    //    return new Index2DFromCenter(a.x, a.y);
    //}

    // It's still a good practice to override Equals and GetHashCode
    public override bool Equals(object obj) {
        if (!(obj is Index2D))
            return false;

        Index2D other = (Index2D)obj;
        return this == other;
    }
    public override int GetHashCode() { return (x, y).GetHashCode(); }

    public static Index2DFromCenter ConvertJointsIndexToCenter(Index2D index2D) {
        if(!GridsSkeleton.Instance || GridsSkeleton.Instance.jointsSkeletonGrid == null) {
            Debug.LogWarning("Cannot convert Index2D to Index2DFromCenter without an initialized GridsSkeleton.");
            return new Index2DFromCenter(index2D.x, index2D.y);
        }
        //TODO - convert to Index2DFromCenter by subtracting distance from center
        return new Index2DFromCenter();
    }
}


public struct Index2DFromCenter {
    public int x;
    public int y;
    public Index2DFromCenter(int x, int y) {
        this.x = x;
        this.y = y;
    }

    // Overloads the == operator
    public static bool operator ==(Index2DFromCenter a, Index2DFromCenter b) { return a.x == b.x && a.y == b.y; }

    // Overloads the != operator
    public static bool operator !=(Index2DFromCenter a, Index2DFromCenter b) { return !(a == b); }

    //public static implicit operator Index2D(Index2DFromCenter a) {
    //    return new Index2D(a.x, a.y);
    //}

    // It's still a good practice to override Equals and GetHashCode
    public override bool Equals(object obj) {
        if (!(obj is Index2DFromCenter))
            return false;

        Index2DFromCenter other = (Index2DFromCenter)obj;
        return this == other;
    }
    public override int GetHashCode() { return (x, y).GetHashCode(); }
}