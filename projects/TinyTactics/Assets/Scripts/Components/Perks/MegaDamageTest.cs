using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(PlayerUnit))]
public class MegaDamageTest : MonoBehaviour
{
    private PlayerUnit boundUnit;

    void Awake() {
        boundUnit = GetComponent<PlayerUnit>();
    }

    void OnEnable() {
        boundUnit.OnAttack += MegaDamage;
    }

    private void MegaDamage(ref MutableAttack mutAtt, Unit target) {
        mutAtt.damage *= 100;
        mutAtt.hitRate *= 100;
        mutAtt.critRate *= 100;
    }
}