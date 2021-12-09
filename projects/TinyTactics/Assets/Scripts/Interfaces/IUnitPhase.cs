using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitPhase
{
    bool active { get; set; }
    bool moveAvailable { get; set; }
    bool attackAvailable { get; set; }

    void Refresh();
    void TriggerPhase();
    void EndPhase();
    void Finish();
}
