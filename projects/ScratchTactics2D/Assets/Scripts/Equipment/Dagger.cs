using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    public override List<string> tags {
        get => new List<string>{ "pierce" };
    }
    public sealed override int MIGHT    { get => 1; }
    public sealed override int ACCURACY { get => 90; }
    public sealed override int CRITICAL { get => 15; }
    public sealed override int REACH    { get => 2; }

    public sealed override string strScaling { get => "D"; }
    public sealed override string dexScaling { get => "B"; }
}
