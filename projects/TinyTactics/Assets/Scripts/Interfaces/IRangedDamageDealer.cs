using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IRangedDamageDealer
{
    Dictionary<int, float> GetDamageProjection();
}