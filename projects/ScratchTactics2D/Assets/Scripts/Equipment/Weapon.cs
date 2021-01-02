using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Equipment
{
    public abstract int MIGHT    { get; }
    public abstract int ACCURACY { get; }
    public abstract int CRITICAL { get; }
    public abstract int REACH    { get; }
}
