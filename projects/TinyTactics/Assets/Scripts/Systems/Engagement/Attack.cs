using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Attack
{
    public enum AttackType {
        Normal,
        Combo
    };
    public AttackType attackType;

    public enum AttackDirection {
        Normal,
        Counter
    };
    public AttackDirection attackDirection;

    public Unit generator;
    public Unit target;

    // from the aggressor
    public Damage damage;
    public Damage poiseDamage;
    public int critRate;

    // from the defender
    public int damageReduction;
    //
    public List<MutatorDisplayData> attackMutators;
    public List<MutatorDisplayData> defenseMutators;

    public Attack(MutableAttack mutAtt, Unit a, Unit b) {
        // actually copy the units into the attack
        // this is because we want some Units to create multiple attacks
        // from a single Engagement
        generator = a;
        target = b;

        damage = mutAtt.damage;
        poiseDamage = mutAtt.poiseDamage;
        critRate = mutAtt.critRate;
        damageReduction = mutAtt.damageReduction;
    
        attackType = mutAtt.attackType;
        attackDirection = mutAtt.attackDirection;
        //
        attackMutators = new List<MutatorDisplayData>(mutAtt.attackMutators);
        defenseMutators = new List<MutatorDisplayData>(mutAtt.defenseMutators);
    }

    // define our behavior for resolution here
    // for example, we subtract defense after checking crit, meaning
    // crits can be used to "pierce armor"
    public AttackResolution Resolve() {
        int baseDamageDealt = damage.Resolve();
        int poiseDamageDealt = poiseDamage.Resolve();

        bool isCrit = Random.Range(0, 100) < critRate;
        int outgoingDamage = (isCrit) ? baseDamageDealt*2 : baseDamageDealt;
        int damageDealt = (int)Mathf.Max(0, outgoingDamage - damageReduction);

        // written this way to reserve space for firing an optional event
        MutableAttackResolution mutableAttackResolution = new MutableAttackResolution(damageDealt, poiseDamageDealt, isCrit);
        return new AttackResolution(mutableAttackResolution);
    }

    public static Attack GenerateAttack(Unit generator, Unit target, AttackType aType, AttackDirection aDirection) {
        Damage attackDamage = (aType == AttackType.Normal) ? new Damage(generator.EquippedWeapon.DamageRange) : new Damage(generator.EquippedWeapon.COMBO_ATK);
        int attackCrit = (aType == AttackType.Normal) ? generator.EquippedWeapon.CRIT : generator.EquippedWeapon.COMBO_CRIT;
        MutableAttack mutableAttack = new MutableAttack(
            // from attacker
            attackDamage,
            new Damage(
                generator.EquippedWeapon.POISE_ATK,
                _damageType: Damage.DamageType.Poise
            ), 
            attackCrit,

            // from defender
            target.statSystem.DAMAGE_REDUCTION,

            // final info
            aType,
            aDirection
        );
        
        // THIS WILL MODIFY THE OUTGOING ATTACK PACKAGE
        generator.FireOnAttackGenerationEvent(ref mutableAttack, target);
        target.FireOnDefenseGenerationEvent(ref mutableAttack, generator);
        return new Attack(mutableAttack, generator, target);
    }

    public static AttackDirection OppositeDirection(AttackDirection ad) {
        if (ad == AttackDirection.Normal) return AttackDirection.Counter;
        else return AttackDirection.Normal;
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableAttack
{
    public Attack.AttackType attackType;
    public Attack.AttackDirection attackDirection;

    // from the aggressor
    public Damage damage;
    public Damage poiseDamage;
    public int critRate;

    // from the defender
    public int damageReduction;
    //
    public List<MutatorDisplayData> attackMutators;
    public List<MutatorDisplayData> defenseMutators;

    public MutableAttack(Damage d, Damage pd, int crit, int dr, Attack.AttackType aType, Attack.AttackDirection aDirection) {
        damage = d;
        poiseDamage = pd;
        critRate = crit;
        damageReduction = dr;

        attackType = aType;
        attackDirection = aDirection;
        //
        attackMutators = new List<MutatorDisplayData>();
        defenseMutators = new List<MutatorDisplayData>();
    }

    public void AddAttackMutator(IMutatorComponent mc) {
        attackMutators.Add(mc.mutatorDisplayData);
    }

    public void AddDefenseMutator(IMutatorComponent mc) {
        defenseMutators.Add(mc.mutatorDisplayData);
    }   
}