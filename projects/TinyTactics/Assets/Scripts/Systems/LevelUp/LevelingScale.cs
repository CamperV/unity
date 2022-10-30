using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "UnitData/LevelingScale")]
public class LevelingScale : ScriptableObject
{
    public int[] ExpReq;

    public int Match(int experienceValue) {
        int l;
        for (l = 1; l < ExpReq.Length; l++) {
            if (experienceValue < ExpReq[l]) {
                break;
            }
        }
        return l;
    }
}