using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageablePart : MonoBehaviour, IDamageable
{
    public Destructible parentObject;

    public void Break(DamageInstance instance)
    {
        
    }

    public void Damage(DamageInstance instance)
    {
        parentObject.Damage(instance);
    }
}
