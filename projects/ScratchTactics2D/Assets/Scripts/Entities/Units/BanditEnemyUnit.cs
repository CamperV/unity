using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BanditEnemyUnit : EnemyUnit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "BanditEnemyUnit",

                VITALITY  = 20,
                STRENGTH  = 1,
                DEXTERITY = 1,
                REFLEX    = 3,
                MOVE      = 5,

                inventoryPool = new List<Equipment>{
                    new Sword(),
                    new ThrowingDagger()
                }
            }.DeriveRemaining();
        }
    }

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? BanditEnemyUnit.defaultStats;
        set => _unitStats = value;
    }
}
