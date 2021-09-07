using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class UnitClass : MonoBehaviour
{
    // assign in the inspector, and attach this to the affected Unit's Animator
    public abstract RuntimeAnimatorController playerUnitAnimator { get; set; }
    public abstract RuntimeAnimatorController enemyUnitAnimator { get; set; }

    // what are the default stats of a new unit of type "UnitClass"
    // Dictionary must contain:
    //  VITALITY            starting value and upper bound of _HP
    //  STRENGTH            contributes to scaling damage, and carrying capacity
    //  DEXTERITY           contributes to scaling damage, hit rate, and critical rate
    //  REFLEX              contributes to evasion and critical evasion
    //  MOVE                dictates squares a unit can move in a turn
    // public abstract Dictionary<string, int> baseStats { get; }
    // actually is just implemented statically

    // what weapons can a unit of type "UnitClass" wield properly?
    public abstract List<string> weaponProfiencies { get; }

    public static UnitState GenerateDefaultState(Guid ID, string name, string callingClass) {
        UnitState unitState = new UnitState(){ ID = ID, unitName = name };
        unitState.unitClass = callingClass;


        PropertyInfo baseStatsProp = Type.GetType(callingClass).GetProperty("baseStats");
        Dictionary<string, int> baseStats = (Dictionary<string, int>)baseStatsProp.GetValue(null, null);
        unitState.VITALITY  = baseStats["VITALITY"];
        unitState.STRENGTH  = baseStats["STRENGTH"];
        unitState.DEXTERITY = baseStats["DEXTERITY"];
        unitState.REFLEX    = baseStats["REFLEX"];
        unitState.MOVE      = baseStats["MOVE"];

        // populate derivative stats
        unitState._HP = unitState.VITALITY;
        unitState._CAPACITY = unitState.STRENGTH;

        MethodInfo Getter = Type.GetType(callingClass).GetMethod("GetStartingEquipment");
		List<Equipment> startingEquipment = (List<Equipment>)Getter.Invoke(null, null);
        unitState.inventory = new Inventory(startingEquipment);

        return unitState;
    }
}
