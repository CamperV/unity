using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BerserkerClass : UnitClass
{
    public static Dictionary<string, int> baseStats {
        get => new Dictionary<string, int>{
            ["VITALITY"]  = 25,
            ["STRENGTH"]  = 15,
            ["DEXTERITY"] = 5,
            ["REFLEX"]    = 8,
            ["MOVE"]      = 7
        };
    }

    public override RuntimeAnimatorController playerUnitAnimator { get; set; }
    public override RuntimeAnimatorController enemyUnitAnimator { get; set; }
    
    public override List<string> weaponProfiencies {
        get => new List<string>{
            "StrikeWeapon"
        };
    }

    void Awake() {
        enemyUnitAnimator = Resources.Load<RuntimeAnimatorController>("Characters/EnemyBerserker");
    }

    public static List<Equipment> GetStartingEquipment() {
        return new List<Equipment>{
            new Axe()
        };
    }
}
