using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BanditPlayerUnit : PlayerUnit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "BanditPlayerUnit",

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
        get => _unitStats ?? BanditPlayerUnit.defaultStats;
        set => _unitStats = value;
    }
}
