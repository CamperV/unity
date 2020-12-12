using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSkeleton : Unit
{
    public override int movementRange { get { return 4; } }
    public override int attackReach { get { return 1; } }
}
