using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMercenary : Unit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
                unitTag = "UnitMercenary",

                STRENGTH  = 3,
                HEALTH    = 10,
                MAXHEALTH = 10,
                MOVE      = 5,
                RANGE     = 1
            };
        }
    }

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? UnitMercenary.defaultStats;
        set => _unitStats = value;
    }
}
