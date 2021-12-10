using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitPhaseInfo
{
    bool turnActive { get; set; }
    bool moveAvailable { get; set; }
    bool attackAvailable { get; set; }

    void StartTurn();
    void FinishTurn();
}
