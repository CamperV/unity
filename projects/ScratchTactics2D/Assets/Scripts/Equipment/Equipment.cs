using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment
{
    public virtual int weight { get => 1; }
    public bool isEquipped { get; private set; }
    
    public virtual void Equip() {
        isEquipped = true;
    }

    public virtual void Unequip() {
        isEquipped = false;
    }
}
