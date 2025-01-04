using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public interface IWeapon_OLD
{
    public abstract void Initialize();

    public abstract void StartFire();

    public abstract void ReleaseFire();

    public abstract DamageInstance GetDamageInstance(Vector3 point, Vector3 velocity);

    public abstract void PlayHitEffect(Vector3 point);

    public abstract void SetHardpoints(List<WeaponHardpoint> hardpoints);

    public abstract Transform GetRootTransform();

    public abstract void AddEventListener(IWeaponListener_OLD listener);

    public abstract void RemoveEventListener(IWeaponListener_OLD listener);

    public abstract void OnAimAssist(bool assist, Transform target);
}

public interface IWeaponListener_OLD
{
    public abstract void OnAmmoEmpty();
}
