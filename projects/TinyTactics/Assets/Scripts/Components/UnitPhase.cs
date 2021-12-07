using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitPhase : MonoBehaviour
{
    public bool active;
    public bool moveAvailable;
    public bool attackAvailable;

    void Awake() {
        active = false;
        moveAvailable = false;
        attackAvailable = false;
    }

    public void Refresh() {
        active = true;
        moveAvailable = true;
        attackAvailable = true;
    }
}
