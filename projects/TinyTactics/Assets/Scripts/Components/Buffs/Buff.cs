using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// What do buffs do?
// Generally, they are provided by other entities (sometimes Perks),
// but Destroy themselves after conditions are met
public abstract class Buff : ValuedStatus
{
}