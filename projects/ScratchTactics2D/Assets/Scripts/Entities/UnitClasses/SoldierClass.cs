using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierClass : UnitClass
{
    public static Dictionary<string, int> baseStats = new Dictionary<string, int>{
        ["VITALITY"]  = 10,
        ["STRENGTH"]  = 10,
        ["DEXTERITY"] = 10,
        ["REFLEX"]    = 10,
        ["MOVE"]      = 6
    };

    public override RuntimeAnimatorController playerUnitAnimator { get; set; }
    public override RuntimeAnimatorController enemyUnitAnimator { get; set; }
    
    public override List<string> weaponProfiencies {
        get => new List<string>{
            "PierceWeapon"
        };
    }

    void Awake() {
        playerUnitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/AlliedSpearman");
        enemyUnitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/EnemySpearman");
    }

    public static List<Equipment> GetStartingEquipment() {
        return new List<Equipment>{
            new Spear()
        };
    }
}
