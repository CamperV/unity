using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(WeaponStats))]
public abstract class Weapon : MonoBehaviour, ITagged
{
    // set by equipping Unit
    public Unit boundUnit { get; set; }

    // assigned in inspector or otherwise
    public string displayName;
    public Sprite sprite;
    public Color color;
    
    public WeaponStats weaponStats { get; set; }
    [field: SerializeField] public List<string> tags { get; set; }

    // experimental
    public AudioFXBundle audioFXBundle;

    void Awake() {
        weaponStats = GetComponent<WeaponStats>();
    }

    public abstract int CalculateDamage();

    // ITagged
    public bool HasTagMatch(params string[] tagsToCheck) {
        foreach (string tag in tagsToCheck) {
            if (tags.Contains(tag))
                return true;
        }
        return false;
    }

    public virtual void Equip(Unit unit) {
        boundUnit = unit;
        
        foreach (WeaponPerk wp in GetComponents<WeaponPerk>()) {
            wp.OnEquip();
        }
    }
}