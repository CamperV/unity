using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// this class is created for an acutal battle between two Units
public class Engagement
{
    public delegate void EngagementAttackEvent(Unit target, ref List<Attack> attackList);
    public static event EngagementAttackEvent AttacksQueuedAgainst;

    // an Engagement ALWAYS procedes:
    // 1) exhaust attacks
    // 2) exhaust counterattacks
    public List<Attack> attacks;
    public List<Attack> counterAttacks;

    public Unit A; // A is always the initiator
    public Unit B; // B is always the defender

    public Engagement(Unit a, Unit b) {
        A = a;
        B = b;

        attacks = new List<Attack>();
        counterAttacks = new List<Attack>();

        // gather all attacks from A
        // units that can Combo will add them to this List
        for (int s = 0; s < (A.statSystem.MULTISTRIKE+1); s++) {
            attacks.Add( GenerateAttack(A, B) );

            // send out Engagement combo signal here
            // ie, Broadcast to all Units - an Attack has been generated, please contribute if able
            AttacksQueuedAgainst?.Invoke(B, ref attacks);
        }

        if (EngagementSystem.CounterAttackPossible(A, B)) {
            for (int s = 0; s < (B.statSystem.MULTISTRIKE+1); s++) {
                counterAttacks.Add( GenerateAttack(B, A) );
                
                // send out Engagement combo signal here
                // ie, Broadcast to all Units - an Attack has been generated, please contribute if able
                AttacksQueuedAgainst?.Invoke(A, ref counterAttacks);
            }
        }
    }

    private Attack GenerateAttack(Unit generator, Unit receiver) {
        MutableAttack mutableAttack = new MutableAttack(
            new Damage(generator.EquippedWeapon.DamageRange),   // from attacker
            generator.EquippedWeapon.CRITICAL,                  // from attacker
            receiver.statSystem.DAMAGE_REDUCTION                // from defender
        );
        
        // THIS WILL MODIFY THE OUTGOING ATTACK PACKAGE
        generator.FireOnAttackGenerationEvent(ref mutableAttack, receiver);
        receiver.FireOnDefenseGenerationEvent(ref mutableAttack, generator);
        return new Attack(mutableAttack);
    }
}