using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu (menuName = "Items/Weapon")]
public class Weapon : Item, IEquipable
{
    public AudioFXBundle audioFXBundle_Attack;
    public AudioFXBundle audioFXBundle_Equip;

    public int MIN_ATK;
    public int MAX_ATK;
    public int POISE_ATK;
    public int COMBO_ATK;
    public int MIN_RANGE;
    public int MAX_RANGE;
    public int CRITICAL;

    public static int _MAX_DAMAGE_VALUE = 99;

    // use this to differentiate some weapons. For example, a "heavy" weapon might reduce REFLEX
    public List<Mutation> attachedMutations;
    public List<so_Status> attachedStatuses;

    public Pair<int, int> DamageRange => new Pair<int, int>(MIN_ATK, MAX_ATK);

    // IEquipable
    public virtual void Equip(Unit thisUnit) {
        foreach (Mutation mut in attachedMutations) {
            thisUnit.mutationSystem.AddMutation(mut);
        }

        foreach (so_Status status in attachedStatuses) {
            thisUnit.statusSystem.AddStatus(status, so_Status.CreateStatusProviderID(thisUnit, status));
        }
    }

    // IEquipable
    public virtual void Unequip(Unit thisUnit) {
        foreach (Mutation mut in attachedMutations) {
            thisUnit.mutationSystem.RemoveMutation(mut);
        }

        foreach (so_Status status in attachedStatuses) {
            thisUnit.statusSystem.RemoveStatus(so_Status.CreateStatusProviderID(thisUnit, status));
        }
    }

    public string DisplayDamageRange() {
        if (MIN_ATK == MAX_ATK) {
            return $"{MIN_ATK}";
        } else {
            return $"{MIN_ATK} - {MAX_ATK}";
        }
    }
}