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
                STRENGTH  = 30,
                DEXTERITY = 30,
                REFLEX    = 5,
                MOVE      = 7,

                inventoryPool = new List<Equipment>{
                    new Sword()
                }
            }.DeriveRemaining();
        }
    }

    private UnitState _unitState;
    public override UnitState unitState {
        get => _unitState ?? KnightPlayerUnit.defaultState;
        set => _unitState = value;
    }
}
