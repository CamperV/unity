using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightPlayerUnit : PlayerUnit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "KnightPlayerUnit",

                VITALITY  = 18,
                STRENGTH  = 3,
                DEXTERITY = 3,
                REFLEX    = 5,
                MOVE      = 7,

                inventoryPool = new List<Equipment>{
                    new Sword()
                }
            }.DeriveRemaining();
        }
    }

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? KnightPlayerUnit.defaultStats;
        set => _unitStats = value;
    }
}
