using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class UnitState
{
    public Guid ID;

    public string unitTag;  // used to look up the appropriate prefab
    public string unitName;
    public string unitClass;
    public string unitSubclass = null;  // remains null for now

    // Attributes!  
    public int VITALITY;    // starting value and upper bound of _HP
    public int STRENGTH;    // contributes to scaling damage, and carrying capacity
    public int DEXTERITY;   // contributes to scaling damage, hit rate, and critical rate
    public int REFLEX;      // contributes to evasion and critical evasion
    public int MOVE;        // dictates squares a unit can move in a turn

    // Derived Attributes!
    public int _HP;          // tracks current Hit Points/Health
    public int _CAPACITY;    // dictates how much weight a unit can carry at once

    public List<Equipment> inventoryPool; // choose from this pool when spawning to determine your inventory
    public Inventory inventory;

    // use the bare constructor for readability when creating default stats 
    public UnitState() { }
    public override string ToString() {
        return $"UnitState=>{unitName}/{unitTag}/S{STRENGTH}.D{DEXTERITY}.H{_HP}/{VITALITY}.M{MOVE}";
    }
}
