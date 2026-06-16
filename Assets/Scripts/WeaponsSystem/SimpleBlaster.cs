using SpaceGame.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Weapons
{
    public class SimpleBlaster : IWeapon
    {
        public WeaponSystem weaponSystem;
        private WeaponData weaponData;

        public ParticleHitEffect explosion;
        public ParticleHitEffect muzzleFlare;
        public SimpleObjectPool<BaseProjectile> projectilePool;
        public SimpleObjectPool<ImpactDecal> impactDecalPool;

        private int currentAmmo = 0;

        private Action OnAmmoEmpty;

        private bool firing = false;
        private bool waiting = false;

        private Targetable target;
        private bool targetSet = false;

        public SimpleBlaster(WeaponSystem system, WeaponData data)
        {
            weaponSystem = system;
            weaponData = data;
            (muzzleFlare, explosion) = WeaponManager.GetParticleEffects(data);

            projectilePool = WeaponManager.GenerateProjectilePool<BaseProjectile>(data.projectileTemplate, data.maxProjectiles, this, data);
            impactDecalPool = WeaponManager.GeneratePool<ImpactDecal>(data.blastMark, data.maxProjectiles);
        }

        public void OnTriggerHold()
        {
            FiringSequence();
        }

        public void OnTriggerRelease()
        {
            firing = false;
        }

        public int GetAmmo()
        {
            return currentAmmo;
        }

        public void AddEventListener(IWeaponListener listener)
        {
            OnAmmoEmpty += listener.OnAmmoEmpty;
        }

        public void RemoveEventListener(IWeaponListener listener)
        {
            OnAmmoEmpty -= listener.OnAmmoEmpty;
        }

        private async void FiringSequence()
        {
            firing = true;
            if (!waiting)
            {
                while (firing)
                {
                    //weaponSystem.Fire(projectilePool.Get(), muzzleFlare);
                    FireProjectile();

                    waiting = true;
                    await Task.Delay(weaponData.firingDelay);
                    waiting = false;
                }
            }
        }

        private void FireProjectile()
        {
            WeaponHardpoint hardpoint = weaponSystem.GetWeaponHardpoint();
            Vector3 dir = weaponSystem.GetFiringDirection(hardpoint.transform, weaponData);
            BaseProjectile proj = projectilePool.Get();

            if (targetSet)
            {
                proj.Launch(hardpoint.transform.position, dir, target);
            }
            else
            {
                proj.Launch(hardpoint.transform.position, dir, weaponSystem.GetDefaultAimPoint());
            }

            //projectilePool.Get().Launch(hardpoint.transform.position, dir, weaponSystem.GetDefaultAimPoint());
            muzzleFlare.PlayAtLocation(hardpoint.transform.position, hardpoint.transform.rotation);
            hardpoint.PlayAnimation();
        }

        public void PlayHitEffect(Vector3 point)
        {
            explosion.PlayAtLocation(point);
        }

        public void ApplyBlastMark(RaycastHit hit)
        {
            float size = UnityEngine.Random.Range(weaponData.blastMarkSizeRange.x, weaponData.blastMarkSizeRange.y);

            impactDecalPool.Get().Apply(size, hit);
        }

        public Transform GetRootTransform()
        {
            return weaponSystem.GetRootTransform();
        }

        public void OnSystemDestroyed()
        {
            firing = false;
        }

        void IWeapon.OnWeaponSwapped()
        {
            throw new NotImplementedException();
        }

        void IWeapon.AssignTarget(Targetable target)
        {
            this.target = target;
            targetSet = true;
        }

        void IWeapon.ClearTargets()
        {
            targetSet = false;
        }
    }
}