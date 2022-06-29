using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject, ITagged
{
    // assigned in inspector or otherwise
    public new string name;
    public Sprite sprite;

    public AudioFXBundle audioFXBundle;

    public int MIN_MIGHT;
    public int MAX_MIGHT;
    public int CRITICAL;
    public int MIN_RANGE;
    public int MAX_RANGE;

    public static int _MAX_DAMAGE_VALUE = 99;

    // use this to differentiate some weapons. For example, a "heavy" weapon might reduce REFLEX
    public List<Mutation> attachedMutations;
    public List<so_Status> attachedStatuses;

    // ITagged
    [field: SerializeField] public List<string> tags { get; set; }

    public bool HasTagMatch(params string[] tagsToCheck) {
        foreach (string tag in tagsToCheck) {
            if (tags.Contains(tag))
                return true;
        }
        return false;
    }

    public void Equip(Unit thisUnit) {
        foreach (Mutation mut in attachedMutations) {
            thisUnit.mutationSystem.AddMutation(mut);
        }

        foreach (so_Status status in attachedStatuses) {
            thisUnit.statusSystem.AddStatus(status);
        }
    }

    public void Unequip(Unit thisUnit) {
        foreach (Mutation mut in attachedMutations) {
            thisUnit.mutationSystem.AddMutation(mut);
        }

        foreach (so_Status status in attachedStatuses) {
            thisUnit.statusSystem.AddStatus(status);
        }   
    }

    public Pair<int, int> DamageRange(Unit thisUnit) {
        int upper = Mathf.Clamp(thisUnit.unitStats.STRENGTH + MAX_MIGHT, 0, _MAX_DAMAGE_VALUE);
        int lower = Mathf.Clamp(thisUnit.unitStats.STRENGTH + MIN_MIGHT, 0, upper);
        return new Pair<int, int>(lower, upper);
    }
}