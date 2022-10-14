using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int _x, int _y) { x = _x; y = _y; }
    public GridPosition(Vector3Int v) { x = v.x; y = v.y; }
    public GridPosition(Vector2Int v) { x = v.x; y = v.y; }

    public bool Equals(GridPosition other) {
        return x == other.x && y == other.y;
    }

    public override int GetHashCode() {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }

    public override string ToString() {
        return $"[{x}, {y}]";
    }

    public static GridPosition up => new GridPosition(0, 1);
    public static GridPosition right => new GridPosition(1, 0);
    public static GridPosition down => new GridPosition(0, -1);
    public static GridPosition left => new GridPosition(-1, 0);

    // public int CompareTo(object obj) {
    //     if (!(obj is GridPosition)) {
    //         throw new ArgumentException($"Cannot compare GridPosition to {typeof(obj)}");

    //     } else {
    //         GridPosition other = obj as GridPosition;

    //     }
    // }

    /////////////////////////////////
    // accessible useful functions //
    /////////////////////////////////
    public int ManhattanDistance(GridPosition o) {
        return Mathf.Abs(x-o.x) + Mathf.Abs(y-o.y);
    }

    public GridPosition X(int _x) => new GridPosition(_x, this.y);
    public GridPosition Y(int _y) => new GridPosition(this.x, _y);

    // quick scalable approach: iterate through a virtual cube and simply check the bounds and yield if within
    // includes self, and if fully inclusive of min/range
    public IEnumerable<GridPosition> Radiate(int range, int min = 0) {
        GridPosition lower = this - new GridPosition(range, range);
        GridPosition upper = this + new GridPosition(range, range);

        for (int x = lower.x; x <= upper.x; x++) {
            for (int y = lower.y; y <= upper.y; y++) {
                GridPosition gp = new GridPosition(x, y);
                int mDist = ManhattanDistance(gp);
                if (mDist <= range && mDist >= min) yield return gp;
            }
        }
    }

    ///////////////
    // operators //
    ///////////////
    public static bool operator ==(GridPosition gp, GridPosition other) {
        return gp.Equals(other);
    }

    public static bool operator !=(GridPosition gp, GridPosition other) {
        return !gp.Equals(other);
    }

    public static GridPosition operator+(GridPosition a, GridPosition b) {
        return new GridPosition(a.x + b.x, a.y + b.y);
    }

    public static GridPosition operator-(GridPosition a, GridPosition b) {
        return new GridPosition(a.x - b.x, a.y - b.y);
    }

    public static GridPosition operator*(GridPosition a, GridPosition b) {
        return new GridPosition(a.x * b.x, a.y * b.y);
    }

    public static GridPosition operator-(GridPosition a) {
        return new GridPosition(-a.x, -a.y);
    }

    public static GridPosition operator*(GridPosition a, int b) {
        return new GridPosition(a.x * b, a.y * b);
    }

    public static GridPosition operator*(int a, GridPosition b) {
        return new GridPosition(a * b.x, a * b.y);
    }

    public static GridPosition operator/(GridPosition a, int b) {
        return new GridPosition(a.x / b, a.y / b);
    }

    ///////////
    // casts //
    ///////////
    public static implicit operator Vector3Int(GridPosition gp) {
        return new Vector3Int(gp.x, gp.y, 0);
    }

    public static implicit operator GridPosition(Vector3Int v) {
        return new GridPosition(v.x, v.y);
    }

    public static implicit operator Vector2Int(GridPosition gp) {
        return new Vector2Int(gp.x, gp.y);
    }

    public static implicit operator GridPosition(Vector2Int v) {
        return new GridPosition(v.x, v.y);
    }
}