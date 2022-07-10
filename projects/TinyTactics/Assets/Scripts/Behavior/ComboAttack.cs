using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ComboAttack
{
    public int damage;

    public ComboAttack(MutableComboAttack mutCombo) {
        damage = mutCombo.damage;
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableComboAttack
{
    public int damage;

    public MutableComboAttack(int dmg) {
        damage = dmg;
    }
}