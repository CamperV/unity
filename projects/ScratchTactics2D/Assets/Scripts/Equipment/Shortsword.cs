using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortsword : WeaponSlash
{
    public sealed override int MIGHT    { get => 3; }
    public sealed override int ACCURACY { get => 70; }
    public sealed override int CRITICAL { get => 5; }
    public sealed override int REACH    { get => 1; }
}
