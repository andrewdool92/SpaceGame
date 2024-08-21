using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public abstract void Damage(DamageInstance instance);

}

public struct DamageInstance
{
    public float damage;
    public float blastPower;
    public float blastRadius;

    public Vector3 hitPoint;
    public Vector3 hitVelocity;

    public DamageInstance(float damage, float blastPower, float blastRadius, Vector3 hitPoint, Vector3 hitVelocity)
    {
        this.damage = damage;
        this.blastPower = blastPower;
        this.blastRadius = blastRadius;

        this.hitPoint = hitPoint;
        this.hitVelocity = hitVelocity;
    }
}
