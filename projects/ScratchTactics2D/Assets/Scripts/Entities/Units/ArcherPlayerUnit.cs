using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcherPlayerUnit : PlayerUnit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "ArcherPlayerUnit",

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
        get => _unitStats ?? ArcherPlayerUnit.defaultStats;
        set => _unitStats = value;
    }
}
