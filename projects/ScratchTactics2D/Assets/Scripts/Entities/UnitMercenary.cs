using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMercenary : Unit
{
    private int MOVE = 5;
    private int RANGE = 1;
    private int DAMAGE = 2;
    private int HEALTH = 10;

    public override int movementRange   { get => MOVE;      set => MOVE = value; }
    public override int attackReach     { get => RANGE;     set => RANGE = value; }
    public override int damageValue     { get => DAMAGE;    set => DAMAGE = value; }
    public override int maximumHealth   { get => HEALTH;    set => HEALTH = value; }
}
