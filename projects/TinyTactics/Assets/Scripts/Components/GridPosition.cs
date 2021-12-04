using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public interface IGridPosition
{
    GridPosition gridPosition { get; set; }
}

[Serializable]
public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int _x, int _y) { x = _x; y = _y; }
    public GridPosition(Vector3Int v) { x = v.x; y = v.y; }
    public GridPosition(Vector2Int v) { x = v.x; y = v.y; }

    public int ManhattanDistance(GridPosition o) {
        return Mathf.Abs(x-o.x) + Mathf.Abs(y-o.y);
    }

    public bool Equals(GridPosition other) {
        return x == other.x && y == other.y;
    }

    public int GetHashCode() {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }

    public override string ToString() {
        return $"[{x}, {y}]";
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
}