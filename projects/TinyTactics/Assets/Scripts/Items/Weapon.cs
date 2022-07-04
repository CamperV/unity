using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item, IEquipable
{
    public AudioFXBundle audioFXBundle_Attack;
    public AudioFXBundle audioFXBundle_Equip;

    public int MIN_MIGHT;
    public int MAX_MIGHT;
    public int CRITICAL;
    public int MIN_RANGE;
    public int MAX_RANGE;

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

    public Pair<int, int> DamageRange(Unit thisUnit) {
        int upper = Mathf.Clamp(thisUnit.unitStats.STRENGTH + MAX_MIGHT, 0, _MAX_DAMAGE_VALUE);
        int lower = Mathf.Clamp(thisUnit.unitStats.STRENGTH + MIN_MIGHT, 0, upper);
        return new Pair<int, int>(lower, upper);
    }
}