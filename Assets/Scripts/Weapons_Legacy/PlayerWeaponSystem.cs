using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Weapons;

public class PlayerWeaponSystem : MonoBehaviour, IWeaponListener_OLD
{
    [Header("Main weapons")]
    [SerializeField] private List<Blaster> mainWeapons;
    [SerializeField] private List<WeaponHardpoint> weaponHardpoints;

    private int mainEquipIndex = 0;

    [Header("Secondary weapons")]
    

    [SerializeField] private ReticuleController reticule;

    private void Start()
    {
        foreach(IWeapon_OLD weapon in mainWeapons)
        {
            weapon.SetHardpoints(weaponHardpoints);
            weapon.Initialize();
            weapon.AddEventListener(this);
            //reticule.onAimAssist += weapon.OnAimAssist;
        }
    }

    private void OnDestroy()
    {
        foreach(IWeapon_OLD weapon in mainWeapons)
        {
            weapon.RemoveEventListener(this);
            //reticule.onAimAssist -= weapon.OnAimAssist;
        }
    }

    public void OnFiringButtonPressed()
    {
        mainWeapons[mainEquipIndex].StartFire();
    }

    public void OnFiringButtonReleased()
    {
        mainWeapons[mainEquipIndex].ReleaseFire();
    }

    public void OnAmmoEmpty()
    {
        OnFiringButtonReleased();
        mainEquipIndex = 0;
        OnFiringButtonPressed();
    }
}
