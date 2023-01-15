using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EngagementSystem : MonoBehaviour
{
    // the class responsible for actually executing/resolving Engagements
    public static EngagementSystem inst = null; // enforces singleton behavior

    // just a nice flag
	public UnityEvent<Engagement> OnCreateEngagement;

    public ComboSystem comboSystem;
	
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

    public static Engagement CreateEngagement(Unit a, Unit b) {
        Engagement e = new Engagement(a, b);
        EngagementSystem.inst.OnCreateEngagement?.Invoke(e);
        return e;
    }

    public void Resolve(Engagement engagement) => StartCoroutine(_Resolve(engagement));
    private IEnumerator _Resolve(Engagement engagement) {
        resolvedFlag = false;

        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        // empty the entire aggressor Attack queue
        foreach (Attack attack in engagement.attacks) {
            defenderSurvived = ProcessAttack(attack);
            if (!defenderSurvived) break;

            yield return new WaitForSeconds(delayBetweenAttacks);
        }

        // we do the subtraction here because for sure, you are going to perform delayBetweenAttacks
        yield return new WaitForSeconds(delayBeforeCounter - delayBetweenAttacks);

        // // before countering, make sure that there are no ongoing animations
        // yield return new WaitUntil(engagement.A.spriteAnimator.EmptyQueue);
        // yield return new WaitUntil(engagement.B.spriteAnimator.EmptyQueue);
        //

        // if we can counterattack:
        // we check inside the loop, because theoretically a counter attack can drain POISE, which will remove the ability to counter
        if (defenderSurvived) {
            foreach (Attack counterAttack in engagement.counterAttacks) {
                if (counterAttack.target.statSystem.CounterAttackAvailable) {
                    aggressorSurvived = ProcessAttack(counterAttack);
                    if (!aggressorSurvived) break;

                    yield return new WaitForSeconds(delayBetweenAttacks);
                }
            }
        }
        
        yield return new WaitUntil(() => AnimationFinished(engagement.GetUnits()));
        resolvedFlag = true;
    }

    private bool AnimationFinished(IEnumerable<Unit> units) {
        return units.All(u => u.spriteAnimator.DoneAnimating());
    }

    private bool ProcessAttack(Attack attack) {
        Unit A = attack.generator;
        Unit B = attack.target;
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
}