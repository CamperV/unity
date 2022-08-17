using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Weapon : Item, IEquipable
{
    public AudioFXBundle audioFXBundle_Attack;
    public AudioFXBundle audioFXBundle_Equip;

    public int MIN_RANGE;
    public int MAX_RANGE;
    public int CRITICAL;

    public static int _MAX_DAMAGE_VALUE = 99;

    // use this to differentiate some weapons. For example, a "heavy" weapon might reduce REFLEX
    public List<Mutation> attachedMutations;
    public List<so_Status> attachedStatuses;

    // IEquipable
    public void Equip(Unit thisUnit) {
        foreach (Mutation mut in attachedMutations) {
            thisUnit.mutationSystem.AddMutation(mut);
        }

        foreach (so_Status status in attachedStatuses) {
            thisUnit.statusSystem.AddStatus(status, so_Status.CreateStatusProviderID(thisUnit, status));
        }
    }

    // IEquipable
    public void Unequip(Unit thisUnit) {
        foreach (Mutation mut in attachedMutations) {
            thisUnit.mutationSystem.RemoveMutation(mut);
        }

        foreach (so_Status status in attachedStatuses) {
            thisUnit.statusSystem.RemoveStatus(so_Status.CreateStatusProviderID(thisUnit, status));
        }
    }

    public abstract Pair<int, int> DamageRange(Unit thisUnit);
    public abstract int RollDamage(Unit thisUnit);
    public abstract int ComboDamage(Unit thisUnit);
    public abstract string DisplayRawDamage(Unit thisUnit);
    public abstract Dictionary<int, float> GenerateProjection(Unit thisUnit);
}