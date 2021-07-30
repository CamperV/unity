using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightPlayerUnit : PlayerUnit
{
    // default unit stats
    public static UnitState defaultState {
        get {
            return new UnitState {
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

    private UnitState _unitStats;
    public override UnitState unitStats {
        get => _unitStats ?? KnightPlayerUnit.defaultState;
        set => _unitStats = value;
    }
}
