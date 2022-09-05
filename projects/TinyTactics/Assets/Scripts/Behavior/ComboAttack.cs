using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ComboAttack
{
    public Unit unit;
    public int damage;

    public ComboAttack(MutableComboAttack mutCombo) {
        unit = mutCombo.unit;
        damage = mutCombo.damage;
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableComboAttack
{
    public Unit unit;
    public int damage;

    public MutableComboAttack(Unit thisUnit, int dmg) {
        unit = thisUnit;
        damage = dmg;
    }
}