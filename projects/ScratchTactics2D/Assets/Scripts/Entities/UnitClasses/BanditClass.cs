using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BanditClass : UnitClass, IEnemyUnitClass
{
    public static Dictionary<string, int> baseStats {
        get => new Dictionary<string, int>{
            ["VITALITY"]  = 10,
            ["STRENGTH"]  = 10,
            ["DEXTERITY"] = 10,
            ["REFLEX"]    = 10,
            ["MOVE"]      = 6
        };
    }

    // IEnemyUnitClass
    public string assignedBrain { get => "GenericBrain"; }

    public override List<string> weaponProfiencies {
        get => new List<string>{
            "pierce", "slash"
        };
    }

    void Awake() {
        unitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/Bandit");
    }

    public static List<Equipment> GetStartingEquipment() {
        return new List<Equipment>{
            new Dagger()
        };
    }
}
