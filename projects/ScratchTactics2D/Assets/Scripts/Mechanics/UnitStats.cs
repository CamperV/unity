using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitStats
{
    public Guid ID;

    public string unitTag;  // used to look up the appropriate prefab
    public string unitName;

    // stats modifiers - usually, the base class (think UnitMercenary) holds stat values
    // these include move speed, max health, etc
    // this class holds the offsets that can be applied to that base level
    public int STRENGTH;
    public int HEALTH;
    public int MAXHEALTH;
    public int MOVE;
    public int RANGE;
    // etc

    public List<EquipableObject> inventory;

    public UnitStats() { }

    public override string ToString() {
        return $"UnitStats=>{unitName}/{unitTag}/S{STRENGTH}/H{HEALTH}/MH{MAXHEALTH}/M{MOVE}/R{RANGE}";
    }
}
