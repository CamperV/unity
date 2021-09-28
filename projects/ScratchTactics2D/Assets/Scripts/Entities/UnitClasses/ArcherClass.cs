using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcherClass : UnitClass, IEnemyUnitClass
{
    public static Dictionary<string, int> baseStats {
        get => new Dictionary<string, int>{
            ["VITALITY"]  = 40,
            ["STRENGTH"]  = 1,
            ["DEXTERITY"] = 20,
            ["REFLEX"]    = 10,
            ["MOVE"]      = 6
        };
    }

    // IEnemyUnitClass
    public string assignedBrain { get => "GenericBrain"; }
    
    public override List<string> weaponProfiencies {
        get => new List<string>{
            "missile"
        };
    }

    void Awake() {
        unitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/Archer");
    }

    public static List<Equipment> GetStartingEquipment() {
        return new List<Equipment>{
            new Bow()
        };
    }
}
