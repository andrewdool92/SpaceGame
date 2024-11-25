using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public interface IWeapon
    {
        public abstract void OnTriggerHold();
        public abstract void OnTriggerRelease();
        public abstract int GetAmmo();
        public abstract void PlayHitEffect(Vector3 point);
        public abstract void ApplyBlastMark(RaycastHit hit);
        public abstract Transform GetRootTransform();
        public abstract void AddEventListener(IWeaponListener listener);
        public abstract void RemoveEventListener(IWeaponListener listener);
        public abstract void OnSystemDestroyed();
        public abstract void OnWeaponSwapped();
        public abstract void AssignTarget();
        public abstract void ClearTargets();
    }

    public interface IWeaponListener
    {
        public abstract void OnAmmoEmpty();
    }
}