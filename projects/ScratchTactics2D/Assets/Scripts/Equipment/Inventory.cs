using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;

public class Inventory
{
    public int totalWeight;

    private List<Weapon> weapons;
    public Weapon equippedWeapon {
        get => weapons.First(it => it.isEquipped);
    }

    // simple struct to store a Unit's inventory
    public Inventory(List<Equipment> equipment) {
        totalWeight = 0;
        weapons = new List<Weapon>();

        // UH OH: turns out reflection is kinda dumb in C#
        // just go ahead and call the constructors ahead of time
        foreach (var e in equipment) {
            totalWeight += e.weight;
            weapons.Add( (e as Weapon) );
        }

        EquipWeapon(weapons[0]);
    }

    public void EquipWeapon(Weapon w) {
        if (!weapons.Contains(w)) weapons.Add(w);
        weapons.ForEach(it => it.Unequip());

        w.Equip();
    }
}
