using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitController
{
    void RegisterUnit(Unit unit);
    void RefreshUnits();
    List<Unit> GetActiveUnits();
}
