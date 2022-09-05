using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class UIComboAttackDisplay : MonoBehaviour
{
    public abstract void DisplayComboAttacks(List<ComboAttack> comboAttacks);
}