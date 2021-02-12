using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class Controller : PhasedObject
{
	public List<MovingObject> registry;
	public List<MovingObject> activeRegistry {
		get {
			return registry.Where(it => it.IsActive()).ToList();
		}
	}

	protected virtual void Awake() {		
		registry = new List<MovingObject>();
    }
	
	public virtual void Register(MovingObject subject) {
		registry.Add(subject);

		if (subject.transform.parent != null) {
			throw new System.InvalidOperationException($"{subject} is already registered to Controller {subject.transform.parent}, unassign and try again");
		}
		subject.transform.SetParent(transform);
	}
}