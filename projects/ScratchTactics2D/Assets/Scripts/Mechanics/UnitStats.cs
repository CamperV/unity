using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class UnitStats
{
    public Guid ID;

    public string unitTag;  // used to look up the appropriate prefab
    public string unitName;

    // Attributes!  
    public int MAXHP;       // starting value and upper bound of HP
    public int STRENGTH;    // contributes to damage and carrying capacity(?)
    public int DEXTERITY;   // contributes to damage and accuracy
    public int SPEED;       // contributes to evasion and critical
    public int MOVE;        // dictates squares a unit can move in a turn

    // Derived Attributes!
    public int HP;          // tracks current Hit Points/Health
    public int CAPACITY;    // dictates how much weight a unit can carry at once

    public List<Weapon> inventoryPool; // choose from this pool when spawning to determine your inventory
    public Inventory inventory;

    // use the bare constructor for readability when creating default stats 
    public UnitStats() { }

    public override string ToString() {
        return $"UnitStats=>{unitName}/{unitTag}/S{STRENGTH}.D{DEXTERITY}.H{HP}/{MAXHP}.M{MOVE}";
    }

    public UnitStats DeriveRemaining() {
        // after construction, assign the remaining stats
        HP = MAXHP;
        CAPACITY = STRENGTH;

        // this will later be limited by strength
        inventory = new Inventory(inventoryPool.RandomSelections<Weapon>(1));
        return this;
    }
}
