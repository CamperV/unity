using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightClass : UnitClass
{
    public static Dictionary<string, int> baseStats {
        get => new Dictionary<string, int>{
            ["VITALITY"]  = 15,
            ["STRENGTH"]  = 2,
            ["DEXTERITY"] = 10,
            ["REFLEX"]    = 10,
            ["MOVE"]      = 6
        };
    }
    
    public override List<string> weaponProfiencies {
        get => new List<string>{
            "slash"
        };
    }

    void Awake() {
        unitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/Knight");
    }

    public static List<Equipment> GetStartingEquipment() {
        return new List<Equipment>{
            new Sword()
        };
    }
}
