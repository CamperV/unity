using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ComboSystem : MonoBehaviour
{
    private List<Unit> comboUnits;

    void Awake() {
        comboUnits = new List<Unit>();
    }

    void OnEnable() {
        Engagement.AttackGenerated += AddComboAttacks;
    }

    void OnDisable() {
        Engagement.AttackGenerated -= AddComboAttacks;
    }

    public void AddUnit(Unit thisUnit) {
        comboUnits.Add(thisUnit);
    }

    public void RemoveUnit(Unit thisUnit) {
        comboUnits.Remove(thisUnit);
    }
    
    public void AddComboAttacks(Unit aggressor, Unit defender, ref List<Attack> attackList) {
        foreach (Unit unit in comboUnits) {
            if (unit != aggressor && ValidComboTarget(unit, defender)) {
                foreach (Attack comboAttack in unit.GenerateAttacks(defender, Attack.AttackType.Combo, Attack.AttackDirection.Normal)) {
                    attackList.Add(comboAttack);
                }
            }
        }
    }

    private bool ValidComboTarget(Unit unit, Unit target) {
        // don't combo your own allies or yourself
        if (unit == target || unit.GetType() == target.GetType())
            return false;

        TargetRange standing = TargetRange.Standing(unit.gridPosition, unit.EquippedWeapon.MIN_RANGE, unit.EquippedWeapon.MAX_RANGE);
        return standing.ValidTarget(target.gridPosition);
    }
}