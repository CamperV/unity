using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilum : WeaponPierce
{
    public sealed override int MIGHT    { get => 4; }
    public sealed override int ACCURACY { get => 60; }
    public sealed override int CRITICAL { get => 5; }
    public sealed override int REACH    { get => 2; }

    public sealed override string strScaling { get => "C"; }
    public sealed override string dexScaling { get => "C"; }
}
