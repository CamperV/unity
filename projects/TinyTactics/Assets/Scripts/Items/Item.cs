using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Item : ScriptableObject, ITagged
{
    // assigned in inspector or otherwise
    public new string name;
    public Sprite sprite;

    // ITagged
    [field: SerializeField] public List<string> tags { get; set; }

    public bool HasTagMatch(params string[] tagsToCheck) {
        foreach (string tag in tagsToCheck) {
            if (tags.Contains(tag))
                return true;
        }
        return false;
    }
}