using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : Weapon
{
    public override List<string> tags {
        get => new List<string>{ "strike" };
    }
    public sealed override int MIGHT    { get => 5; }
    public sealed override int ACCURACY { get => 50; }
    public sealed override int CRITICAL { get => 0; }
    public sealed override int REACH    { get => 1; }

    public sealed override string strScaling { get => "B"; }
    public sealed override string dexScaling { get => "D"; }
}
