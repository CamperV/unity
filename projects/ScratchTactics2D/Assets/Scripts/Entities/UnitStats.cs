using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitStats
{
    public Guid id;

    public string unitTag;  // used to look up the appropriate prefab
    public string unitName;

    // stats modifiers - usually, the base class (think UnitMercenary) holds stat values
    // these include move speed, max health, etc
    // this class holds the offsets that can be applied to that base level
    public int moveMod;
    public int defMod;
    public int maximumHealth;
    public int currentHealth;
    // etc

    public List<EquipableObject> inventory;

    public UnitStats(string tag) {
        id = Guid.NewGuid();
        //
        unitTag = tag;
        unitName = "Jeremy";

        moveMod = 0;
        defMod = 0;

        currentHealth = -1;
    }
}
