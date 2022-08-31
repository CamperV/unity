using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class UIMutatorDisplay : MonoBehaviour
{
    public abstract void DisplayMutators(List<string> mutators);
}