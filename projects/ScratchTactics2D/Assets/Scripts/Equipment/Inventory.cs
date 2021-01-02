using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class Inventory
{
    public int totalWeight;

    private List<Weapon> weapons;
    public Weapon equippedWeapon {
        get => weapons[0];
    }

    // simple struct to store a Unit's inventory
    public Inventory(List<Weapon> equipment) {
        totalWeight = 0;
        weapons = new List<Weapon>();

        // UH OH: turns out reflection is kinda dumb in C#
        // just go ahead and call the constructors ahead of time
        foreach (var e in equipment) {
            totalWeight += e.weight;
            weapons.Add(e);
        }
    }

    public void EquipWeapon(Weapon w) {
        weapons[0] = w;
    }
}
