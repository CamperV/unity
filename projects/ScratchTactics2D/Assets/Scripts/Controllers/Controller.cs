using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class Controller : MonoBehaviour
{
	public List<MovingGridObject> registry = new List<MovingGridObject>();
	public List<MovingGridObject> activeRegistry {
		get {
			return registry.Where(it => it.IsActive()).ToList();
		}
	}
	
	public virtual void Register(MovingGridObject subject) {
		registry.Add(subject);

		if (subject.transform.parent != null) {
			throw new System.InvalidOperationException($"{subject} is already registered to Controller {subject.transform.parent}, unassign and try again");
		}
		subject.transform.SetParent(transform);
	}
}