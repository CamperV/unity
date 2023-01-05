using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IMutatorComponent
{
	MutatorDisplayData mutatorDisplayData { get; set; }
}

[Serializable]
public struct MutatorDisplayData
{
	public new string name;
	public string description;
}