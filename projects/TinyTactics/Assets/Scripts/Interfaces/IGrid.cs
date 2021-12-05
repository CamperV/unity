using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IGrid<T> where T : struct
{
	T WorldToGrid(Vector3 worldPosition);
    Vector3 GridToWorld(T gridPosition);

    void TintTile(T gridPosition, Color color);
    void ResetTintTile(T gridPosition);
}