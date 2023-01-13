using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EngagementSystem : MonoBehaviour
{
    // the class responsible for actually executing/resolving Engagements
    public static EngagementSystem inst = null; // enforces singleton behavior
	
    void Awake() {
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
    }

    [SerializeField] private float delayBetweenAttacks = 0.5f;
    [SerializeField] private float delayBeforeCounter = 1f;
    private bool resolvedFlag = false;

    public void Resolve(Engagement engagement) => StartCoroutine(_Resolve(engagement));
    private IEnumerator _Resolve(Engagement engagement) {
        resolvedFlag = false;

        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        // empty the entire aggressor Attack queue
        foreach (Attack attack in engagement.attacks) {
            defenderSurvived = ProcessAttack(engagement.A, engagement.B, attack);
            if (!defenderSurvived) break;

            yield return new WaitForSeconds(delayBetweenAttacks);
        }

        // we do the subtraction here because for sure, you are going to perform delayBetweenAttacks
        yield return new WaitForSeconds(delayBeforeCounter - delayBetweenAttacks);

        // before countering, make sure that there are no ongoing animations
        yield return new WaitUntil(engagement.A.spriteAnimator.EmptyQueue);
        yield return new WaitUntil(engagement.B.spriteAnimator.EmptyQueue);
        //

        // if we can counterattack:
        // we check inside the loop, because theoretically a counter attack can drain POISE, which will remove the ability to counter
        if (defenderSurvived) {
            foreach (Attack counterAttack in engagement.counterAttacks) {
                if (engagement.B.statSystem.CounterAttackAvailable) {
                    aggressorSurvived = ProcessAttack(engagement.B, engagement.A, counterAttack);
                    if (!aggressorSurvived) break;

                    yield return new WaitForSeconds(delayBetweenAttacks);
                }
            }
        }
        
        yield return new WaitUntil(() => AnimationFinished(engagement.A, engagement.B) );
        resolvedFlag = true;
    }

    private bool AnimationFinished(Unit A, Unit B) {
        return A.spriteAnimator.DoneAnimating() && B.spriteAnimator.DoneAnimating();
    }

    private bool ProcessAttack(Unit A, Unit B, Attack attack) {
        AttackResolution attackResolution = attack.Resolve();
        
        A.TriggerAttackAnimation(B.gridPosition);       

        // now the theatrics
        A.personalAudioFX.PlayWeaponAttackFX();
       
        // if the hit is... unimpressive, play a clang or something
        if (attackResolution.damageDealt < 1) B.personalAudioFX.PlayBlockFX();

        // hit/crit
        if (attackResolution.isCrit) {
            A.FireOnCriticalTargetEvent(B);
            A.personalAudioFX.PlayCriticalFX();        
        }

        // then real damage
        // ouchies, play the animations for hurt
        bool survived = B.SufferDamage(attackResolution.damageDealt, A.gameObject, isCritical: attackResolution.isCrit);
        if (survived) {
            B.FireOnHurtByTargetEvent(A);
        } else {
            A.FireOnDefeatTargetEvent(B);
        }

        // then do poise damage
        B.SufferPoiseDamage(attackResolution.poiseDamageDealt, A.gameObject);
        
        // fire the event after suffering damage, so the animations are queued in the right order
        // this also means you will not be debuffed or anything if you die
        A.FireOnHitTargetEvent(B);

		return survived;
	}

    public void ExecuteAfterResolving(Action VoidAction) => StartCoroutine(_ExecuteAfterResolving(VoidAction));
	private IEnumerator _ExecuteAfterResolving(Action VoidAction) {
        yield return new WaitUntil(() => resolvedFlag == true);
		VoidAction();
	}

    // private bool ProcessCombo(Unit A, Unit B, ComboAttack _combo, Defense previousDefense) {        
    //     A.TriggerAttackAnimation(B.gridPosition);

    //     // now the theatrics
    //     A.personalAudioFX.PlayWeaponAttackFX();

    //     int sufferedDamage = (int)Mathf.Clamp((_combo.damage - previousDefense.damageReduction), 0f, 99f);
       
    //     // if the hit is... unimpressive, play a clang or something
    //     if (sufferedDamage < 1) B.personalAudioFX.PlayBlockFX();

    //     // ouchies, play the animations for hurt
    //     bool survived = B.SufferDamage(sufferedDamage, A.gameObject);
    //     if (survived) B.FireOnHurtByTargetEvent(A);
        
    //     // fire the event after suffering damage, so the animations are queued in the right order
    //     // this also means you will not be debuffed or anything if you die
    //     A.FireOnHitTargetEvent(B);

	// 	return survived;
	// }



    public static bool CounterAttackPossible(Unit agg, Unit def) {
        TargetRange defenderTargetRange = TargetRange.Standing(
            def.gridPosition,
            def.EquippedWeapon.MIN_RANGE,
            def.EquippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(agg.gridPosition) && def.statSystem.CounterAttackAvailable;
    }

    public static bool CounterAttackPossible(Unit agg, Unit def, GridPosition fromPosition) {
        TargetRange defenderTargetRange = TargetRange.Standing(
            def.gridPosition,
            def.EquippedWeapon.MIN_RANGE,
            def.EquippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(fromPosition) && def.statSystem.CounterAttackAvailable;
    }
}