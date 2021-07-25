using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MissileWeapon
{
    public sealed override int MIGHT    { get => 1; }
    public sealed override int ACCURACY { get => 110; }
    public sealed override int CRITICAL { get => 5; }
    public sealed override int REACH    { get => 5; }

    public sealed override string strScaling { get => "B"; }
    public sealed override string dexScaling { get => "B"; }
}
