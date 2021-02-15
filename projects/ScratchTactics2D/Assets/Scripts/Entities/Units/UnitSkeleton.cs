﻿using System.Collections;
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

                VITALITY  = 6,
                STRENGTH  = 1,
                DEXTERITY = 1,
                REFLEX    = 3,
                MOVE      = 4,

                inventoryPool = new List<Equipment>{
                    new Shortsword(),
                    new Pilum(),
                    new Mace()
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