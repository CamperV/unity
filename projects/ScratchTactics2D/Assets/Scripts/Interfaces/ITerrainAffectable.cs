using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface ITerrainAffectable
{
	void OnEnterTerrain(Terrain terrain);
}