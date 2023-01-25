using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public abstract class MapItem : MonoBehaviour, IGridPosition
{
    [field: SerializeField] public GridPosition gridPosition { get; set; }

    public abstract void OnUnitEnter(Unit unit);
}
