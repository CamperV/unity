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

                STRENGTH  = 1,
                HEALTH    = 5,
                MAXHEALTH = 5,
                MOVE      = 4,
                RANGE     = 2
            };
        }
    }

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? UnitSkeleton.defaultStats;
        set => _unitStats = value;
    }
}
