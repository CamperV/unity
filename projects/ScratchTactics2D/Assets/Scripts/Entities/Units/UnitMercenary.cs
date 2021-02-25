using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMercenary : PlayerUnit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "UnitMercenary",

                VITALITY  = 10,
                STRENGTH  = 3,
                DEXTERITY = 3,
                REFLEX    = 5,
                MOVE      = 5,

                inventoryPool = new List<Equipment>{
                    new Shortsword(),
                    new ThrowingDagger(),
                    new Pilum(),
                    new Mace()
                }
            }.DeriveRemaining();
        }
    }

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? UnitMercenary.defaultStats;
        set => _unitStats = value;
    }

    private Sprite _portrait;
    public override Sprite portrait {
        get => ResourceLoader.GetSprite("rogue_portrait");
    }
}
