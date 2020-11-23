using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : PhasedObject
{
	protected List<MovingObject> registry;

	protected void Awake() {		
		registry = new List<MovingObject>();
    }
	
	public void Register(MovingObject subject) {
		registry.Add(subject);

		if (subject.transform.parent != null) {
			throw new System.InvalidOperationException($"{subject} is already registered to Controller {subject.transform.parent}, unassign and try again");
		}
		subject.transform.parent = transform;
	}
}