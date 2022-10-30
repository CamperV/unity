using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitData/MutationArchetype")]
public class MutationArchetype : ScriptableObject
{
	private List<Mutation> _mutPoolCache = new List<Mutation>();

    public string name;
    public Color color;

	public IEnumerable<Mutation> GetPool() {
		if (_mutPoolCache.Count == 0) {
			foreach (Mutation loadedData in Resources.LoadAll<Mutation>("ScriptableObjects/Mutations")) {
				if (loadedData.archetype == this) {
					_mutPoolCache.Add(loadedData);
				}
			}
		}

		foreach (Mutation mut in _mutPoolCache) yield return mut;
	}
}