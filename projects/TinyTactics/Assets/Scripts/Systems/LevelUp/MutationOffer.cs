using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Extensions;

[CreateAssetMenu(menuName = "UnitData/MutationOffer")]
public class MutationOffer : LevelUp
{
    public int numOnOffer;

    // based on the archetypes that the Unit belongs to, create an offer  
    public override void Apply(Unit thisUnit) {
        // disable playerUnitController
        // enable menuUnitController
        // get offers
        // create panel
        // wait until one is chosen
        // go forth
        List<Mutation> offering = GetOffering(thisUnit);
        Mutation pickedRandomly = offering.SelectRandom<Mutation>();
        Debug.Log($"Selected this new mutation {pickedRandomly}");

        thisUnit.mutationSystem.AddMutation(pickedRandomly);
    }

    private List<Mutation> GetOffering(Unit thisUnit) {
        List<Mutation> offerings = new List<Mutation>();

        foreach (MutationArchetype mutArch in thisUnit.mutArchetypes) {
            foreach (Mutation mut in mutArch.GetPool()) {
                offerings.Add(mut);
            }
        }

        // prune em down
        offerings.RandomSelectionsUpTo<Mutation>(numOnOffer);
        return offerings;
    }
}