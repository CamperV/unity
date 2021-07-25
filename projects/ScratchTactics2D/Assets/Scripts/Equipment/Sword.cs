using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : SlashWeapon
{
    public sealed override int MIGHT    { get => 3; }
    public sealed override int ACCURACY { get => 70; }
    public sealed override int CRITICAL { get => 5; }
    public sealed override int REACH    { get => 1; }

    public sealed override string strScaling { get => "C"; }
    public sealed override string dexScaling { get => "C"; }
}
