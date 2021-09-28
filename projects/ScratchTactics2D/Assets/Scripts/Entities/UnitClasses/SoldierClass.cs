using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierClass : UnitClass
{
    public static Dictionary<string, int> baseStats {
        get => new Dictionary<string, int>{
            ["VITALITY"]  = 20,
            ["STRENGTH"]  = 2,
            ["DEXTERITY"] = 10,
            ["REFLEX"]    = 10,
            ["MOVE"]      = 6
        };
    }
    
    public override List<string> weaponProfiencies {
        get => new List<string>{
            "pierce"
        };
    }

    void Awake() {
        unitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/Soldier");
    }

    public static List<Equipment> GetStartingEquipment() {
        return new List<Equipment>{
            new Spear()
        };
    }
}
