using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSystem : MonoBehaviour, IWeaponListener
{
    [SerializeField] private List<Blaster> mainWeapons;
    [SerializeField] private List<WeaponHardpoint> weaponHardpoints;

    private int mainEquipIndex = 0;

    private void Start()
    {
        foreach(IWeapon weapon in mainWeapons)
        {
            weapon.SetHardpoints(weaponHardpoints);
            weapon.Initialize();
            weapon.AddEventListener(this);
        }
    }

    private void OnDestroy()
    {
        foreach(IWeapon weapon in mainWeapons)
        {
            weapon.RemoveEventListener(this);
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
