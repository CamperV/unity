using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UnitLevelUpSystem : MonoBehaviour
{
    public void CatchLevelUp(Unit unit) {
        Debug.Log($"Saw {unit} level up");
    }
}