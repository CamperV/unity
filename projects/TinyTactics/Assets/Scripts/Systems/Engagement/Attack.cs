using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Attack
{
    // from the aggressor
    public Damage damage;
    public Damage poiseDamage;
    public int critRate;

    // from the defender
    public int damageReduction;
    //
    public List<MutatorDisplayData> attackMutators;
    public List<MutatorDisplayData> defenseMutators;

    public Attack(MutableAttack mutAtt) {
        damage = mutAtt.damage;
        poiseDamage = mutAtt.poiseDamage;
        critRate = mutAtt.critRate;
        damageReduction = mutAtt.damageReduction;
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

        MutableAttackResolution mutableAttackResolution = new MutableAttackResolution(damageDealt, poiseDamageDealt, isCrit);
        return new AttackResolution(mutableAttackResolution);
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableAttack
{
    // from the aggressor
    public Damage damage;
    public Damage poiseDamage;
    public int critRate;

    // from the defender
    public int damageReduction;
    //
    public List<MutatorDisplayData> attackMutators;
    public List<MutatorDisplayData> defenseMutators;

    public MutableAttack(Damage d, int crit, int dr) {
        damage = d;
        poiseDamage = new Damage(1);
        critRate = crit;
        damageReduction = dr;
        //
        attackMutators = new List<MutatorDisplayData>();
        defenseMutators = new List<MutatorDisplayData>();
    }
    public MutableAttack(Damage d, Damage pd, int crit, int dr) {
        damage = d;
        poiseDamage = pd;
        critRate = crit;
        damageReduction = dr;
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