using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Specialist : Perk, IToolTip
{
    public override string displayName { get; set; } = "Weapon Advantage+ (Specialist)";

    // IToolTip
    public string tooltipName { get; set; } = "Specialist";
    public string tooltip => "Double all Weapon Advantage bonuses.";

    private int multiplier = 2;

    public override void OnAcquire() {
        // boundUnit.OnAttack += DisplayAttack;
        // boundUnit.OnDefend += DisplayDefense;

        AntiPierce api = boundUnit.equippedWeapon.GetComponent<AntiPierce>();
        if (api != null) {
            api.dmgModRate *= multiplier;
            api.hitModRate *= multiplier;
            api.critModRate *= multiplier;
            //
            api.displayName = displayName;
        }

        AntiStrike ast = boundUnit.equippedWeapon.GetComponent<AntiStrike>();
        if (ast != null) {
            ast.dmgModRate *= multiplier;
            ast.hitModRate *= multiplier;
            ast.critModRate *= multiplier;
            //
            ast.displayName = displayName;
        }

        AntiSlash  asl = boundUnit.equippedWeapon.GetComponent<AntiSlash>();
        if (asl != null) {
            asl.dmgModRate *= multiplier;
            asl.hitModRate *= multiplier;
            asl.critModRate *= multiplier;
            //
            asl.displayName = displayName;
        }
    }

    public override void OnRemoval() {
        // boundUnit.OnAttack -= DisplayAttack;
        // boundUnit.OnDefend -= DisplayDefense;

        AntiPierce api = boundUnit.equippedWeapon.GetComponent<AntiPierce>();
        if (api != null) {
            api.dmgModRate /= multiplier;
            api.hitModRate /= multiplier;
            api.critModRate /= multiplier;
        }

        AntiStrike ast = boundUnit.equippedWeapon.GetComponent<AntiStrike>();
        if (ast != null) {
            ast.dmgModRate /= multiplier;
            ast.hitModRate /= multiplier;
            ast.critModRate /= multiplier;
        }

        AntiSlash  asl = boundUnit.equippedWeapon.GetComponent<AntiSlash>();
        if (asl != null) {
            asl.dmgModRate /= multiplier;
            asl.hitModRate /= multiplier;
            asl.critModRate /= multiplier;
        }
    }

    // private void DisplayAttack(ref MutableAttack mutAtt, Unit target) {
    //     if (mutAtt.mutators.Contains("Weapon Advantage")) {
    //         mutAtt.AddMutator(this);
    //     }
    // }

    // private void DisplayDefense(ref MutableDefense mutDef, Unit target) {
    //     if (mutDef.mutators.Contains("Weapon Advantage")) {
    //         mutDef.AddMutator(this);
    //     }
    // }
}