using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public abstract class Terrain
{
	public virtual int occlusion { get => 0; }
	public abstract void Apply(WorldGrid grid);
}