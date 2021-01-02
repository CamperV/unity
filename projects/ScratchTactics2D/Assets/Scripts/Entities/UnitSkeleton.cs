using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSkeleton : Unit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "UnitSkeleton",

                MAXHP     = 5,
                STRENGTH  = 1,
                DEXTERITY = 1,
                SPEED     = 3,
                MOVE      = 4,

                inventoryPool = new List<Weapon>{
                    new Shortsword()
                }
            }.DeriveRemaining();
        }
    }

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? UnitSkeleton.defaultStats;
        set => _unitStats = value;
    }
}
