using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingLauncher : MonoBehaviour, IWeapon_OLD
{
    public void AddEventListener(IWeaponListener_OLD listener)
    {
        throw new System.NotImplementedException();
    }

    public DamageInstance GetDamageInstance(Vector3 point, Vector3 velocity)
    {
        throw new System.NotImplementedException();
    }

    public Transform GetRootTransform()
    {
        throw new System.NotImplementedException();
    }

    public void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public void OnAimAssist(bool assist, Transform target)
    {
        throw new System.NotImplementedException();
    }

    public void PlayHitEffect(Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    public void ReleaseFire()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveEventListener(IWeaponListener_OLD listener)
    {
        throw new System.NotImplementedException();
    }

    public void SetHardpoints(List<WeaponHardpoint> hardpoints)
    {
        throw new System.NotImplementedException();
    }

    public void StartFire()
    {
        throw new System.NotImplementedException();
    }
}
