using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class UIDamageProjector : MonoBehaviour
{
    public abstract void DisplayDamageProjection(EngagementStats engagementProjection);
    public virtual void DisplayDamageProjection(EngagementStats engagementProjection, int multistrikeValue){}
}