using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugStateLabel : MonoBehaviour
{
    public TextMeshPro label;

    public void SetText(string message) => label.SetText(message);
}
