using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BerserkerClass : UnitClass, IEnemyUnitClass
{
    public static Dictionary<string, int> baseStats {
        get => new Dictionary<string, int>{
            ["VITALITY"]  = 25,
            ["STRENGTH"]  = 5,
            ["DEXTERITY"] = 5,
            ["REFLEX"]    = 8,
            ["MOVE"]      = 7
        };
    }

    // IEnemyUnitClass
    public string assignedBrain { get => "GenericBrain"; }

    public override List<string> weaponProfiencies {
        get => new List<string>{
            "strike"
        };
    }

    void Awake() {
        unitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/Berserker");
    }

    public static List<Equipment> GetStartingEquipment() {
        return new List<Equipment>{
            new Axe()
        };
    }
}
