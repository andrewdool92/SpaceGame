using SpaceGame.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        private static WeaponManager instance;
        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        public static Action<float> ProjectileUpdate;
        private void FixedUpdate()
        {
            ProjectileUpdate?.Invoke(Time.deltaTime);
        }

        private Dictionary<WeaponData, (ParticleHitEffect, ParticleHitEffect)> effects;

        public static (ParticleHitEffect, ParticleHitEffect) GetParticleEffects(WeaponData weaponData)
        {
            if (instance.effects == null)
            {
                instance.effects = new();
            }

            if (!instance.effects.ContainsKey(weaponData))
            {
                ParticleHitEffect muzzleFlare = Instantiate(weaponData.muzzleFlare, instance.transform);
                ParticleHitEffect explosion = Instantiate(weaponData.explosion, instance.transform);
                instance.effects[weaponData] = (muzzleFlare, explosion);
            }
            return instance.effects[weaponData];
        }

        public static ObjectPool<T> GenerateProjectilePool<T>(T template, int size, IWeapon parent, WeaponData weaponData) where T : BaseProjectile
        {
            ObjectPool<T> pool = new ObjectPool<T>();

            for (int i = 0; i < size; i++)
            {
                T projectile = Instantiate(template.gameObject).GetComponent<T>();
                projectile.SetLauncher(parent, weaponData);
                pool.Add(projectile);
            }

            return pool;
        }

        public static ObjectPool<T> GeneratePool<T>(T template, int size) where T : MonoBehaviour
        {
            ObjectPool<T> pool = new ObjectPool<T>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(template.gameObject);
                pool.Add(obj.GetComponent<T>());
            }

            return pool;
        }

        public static IWeapon InitializeWeapon(WeaponSystem weaponSystem, WeaponData data)
        {
            return SimpleBlaster.Instantiate(weaponSystem, data);
        }
    }
}