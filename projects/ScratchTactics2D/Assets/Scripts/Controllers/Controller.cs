using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : PhasedObject
{
	protected List<MovingObject> registry;

	protected void Awake() {		
		registry = new List<MovingObject>();
    }
	
	public virtual void Register(MovingObject subject) {
		registry.Add(subject);
	}
}