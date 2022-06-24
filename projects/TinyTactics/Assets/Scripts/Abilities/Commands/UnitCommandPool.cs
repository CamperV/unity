using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using UnityEngine.UI;

[CreateAssetMenu (menuName = "UnitCommands/UnitCommandPool")]
public class UnitCommandPool : ScriptableObject
{
    // fillable via ScriptableObject interface
    public new string name;
    public List<UnitCommand> unitCommands;
}