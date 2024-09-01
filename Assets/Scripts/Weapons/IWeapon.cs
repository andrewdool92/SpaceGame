using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public abstract void Initialize();

    public abstract void StartFire();

    public abstract void ReleaseFire();

    public abstract DamageInstance GetDamageInstance(Vector3 point, Vector3 velocity);

    public abstract void PlayHitEffect(Vector3 point);

    public abstract void SetHardpoints(List<WeaponHardpoint> hardpoints);

    public abstract Transform GetRootTransform();

    public abstract void AddEventListener(IWeaponListener listener);

    public abstract void RemoveEventListener(IWeaponListener listener);
}

public interface IWeaponListener
{
    public abstract void OnAmmoEmpty();
}
