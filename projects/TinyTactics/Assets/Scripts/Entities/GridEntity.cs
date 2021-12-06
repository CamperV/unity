using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
public abstract class GridEntity : MonoBehaviour, IGridPosition
{
    [field: SerializeField] public GridPosition gridPosition { get; set; }
}
