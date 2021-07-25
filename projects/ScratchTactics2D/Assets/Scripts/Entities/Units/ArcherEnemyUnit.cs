using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcherEnemyUnit : EnemyUnit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "ArcherEnemyUnit",

                VITALITY  = 10,
                STRENGTH  = 3,
                DEXTERITY = 3,
                REFLEX    = 1,
                MOVE      = 4,

                inventoryPool = new List<Equipment>{
                    new Bow()
                }
            }.DeriveRemaining();
        }
    }

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? ArcherEnemyUnit.defaultStats;
        set => _unitStats = value;
    }
}
