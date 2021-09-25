using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Weapon
{
    public override List<string> tags {
        get => new List<string>{ "strike" };
    }
    public sealed override int MIGHT    { get => 4; }
    public sealed override int ACCURACY { get => 65; }
    public sealed override int CRITICAL { get => 10; }
    public sealed override int REACH    { get => 1; }

    public sealed override string strScaling { get => "C"; }
    public sealed override string dexScaling { get => "D"; }
}
