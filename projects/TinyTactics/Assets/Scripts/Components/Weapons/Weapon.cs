using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(WeaponStats))]
public class Weapon : MonoBehaviour
{
    // set by equipping Unit
    public Unit boundUnit { get; set; }

    // assigned in inspector or otherwise
    public string name;
    public Sprite sprite;
    public Color color;
    
    [HideInInspector] public WeaponStats weaponStats;
    public List<string> tags;

    // experimental
    [SerializeField] public AudioFXBundle audioFXBundle;

    void Awake() {
        weaponStats = GetComponent<WeaponStats>();
    }

    public bool HasTagMatch(params string[] tagsToCheck) {
        foreach (string tag in tagsToCheck) {
            if (tags.Contains(tag))
                return true;
        }
        return false;
    }

    public void Equip(Unit unit) {
        boundUnit = unit;
        
        foreach (WeaponPerk wp in GetComponents<WeaponPerk>()) {
            wp.OnEquip();
        }
    }
}