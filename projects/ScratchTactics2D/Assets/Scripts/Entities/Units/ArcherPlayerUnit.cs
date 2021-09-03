using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcherPlayerUnit : PlayerUnit
{
    // default unit stats
    public static UnitState defaultState {
        get {
            return new UnitState {
                unitTag = "ArcherPlayerUnit",

                VITALITY  = 10,
                STRENGTH  = 30,
                DEXTERITY = 30,
                REFLEX    = 1,
                MOVE      = 4,

                inventoryPool = new List<Equipment>{
                    new Bow()
                }
            }.DeriveRemaining();
        }
    }

    private UnitState _unitState;
    public override UnitState unitState {
        get => _unitState ?? ArcherPlayerUnit.defaultState;
        set => _unitState = value;
    }
}
