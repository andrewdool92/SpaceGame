using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoSystem
{
    [SerializeField] private int maxAmmo;
    public int currentAmmo { get; private set; }

    public bool SpendAmmo(int amount)
    {
        if (currentAmmo < amount)
        {
            return false;
        }

        currentAmmo -= amount;
        return (currentAmmo > 0);
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
    }
}
