using SpaceGame.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Weapons
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private Transform rootTransform;
        [SerializeField] private List<WeaponData> weaponData = new List<WeaponData>();
        private int weaponIndex = 0;
        private List<IWeapon> weapons;

        [SerializeField] public List<WeaponHardpoint> hardpoints = new List<WeaponHardpoint>();
        private int hardpointIndex = 0;

        //public List<Targetable> lockTargets;
        public Targetable assistTarget;
        private bool assisting = false;

        [SerializeField] private float assistDegrees;
        private float assistRads;

        private Transform aimTransform;

        private void Start()
        {
            //lockTargets = new();

            if (rootTransform == null)
            {
                rootTransform = transform;
            }

            weapons = new();
            foreach(WeaponData data in weaponData)
            {
                weapons.Add(WeaponManager.InitializeWeapon(this, data));
            }

            assistRads = Mathf.Deg2Rad * assistDegrees;
        }

        private void OnDestroy()
        {
            foreach(IWeapon weapon in weapons)
            {
                weapon.OnSystemDestroyed();
            }
        }

        public WeaponHardpoint GetWeaponHardpoint()
        {
            WeaponHardpoint hardpoint = hardpoints[hardpointIndex];
            hardpointIndex = (hardpointIndex + 1) % hardpoints.Count;
            return hardpoint;
        }

        public void Fire(BaseProjectile projectile, ParticleHitEffect muzzleFlare)
        {
            WeaponHardpoint hardpoint = GetWeaponHardpoint();
            LaunchProjectile(projectile, hardpoint);
            muzzleFlare.PlayAtLocation(hardpoint.transform.position, hardpoint.transform.rotation);
            hardpoint.PlayAnimation();
        }

        public void FireTargeted(BaseProjectile projectile, ParticleHitEffect muzzleFlare, Targetable target, WeaponData weapon)
        {
            WeaponHardpoint hardpoint = GetWeaponHardpoint();
            Vector3 firingDir = GetFiringDirection(hardpoint.transform, weapon);

            projectile.Launch(hardpoint.transform.position, firingDir, target);
            muzzleFlare.PlayAtLocation(hardpoint.transform.position, hardpoint.transform.rotation);
            hardpoint.PlayAnimation();
        }

        //public async void FireMultitarget(ObjectPool<BaseProjectile> projectiles, ParticleHitEffect muzzleFlare)
        //{
        //    WeaponData wpn = weaponData[weaponIndex];
        //    Targetable[] targetSnapshot = new Targetable[lockTargets.Count];
        //    lockTargets.CopyTo(targetSnapshot);
        //    lockTargets.Clear();

        //    foreach(Targetable target in targetSnapshot)
        //    {
        //        FireTargeted(projectiles.Get(), muzzleFlare, target, wpn);
        //        await Task.Delay(wpn.multitargetDelay);
        //    }
        //}

        private void LaunchProjectile(BaseProjectile projectile, WeaponHardpoint hardpoint)
        {
            Vector3 firingDirection = GetFiringDirection(hardpoint.transform, weaponData[weaponIndex]);
            if (assisting)
            {
                projectile.Launch(hardpoint.transform.position, firingDirection, assistTarget);
            }
            else
            {
                projectile.Launch(hardpoint.transform.position, firingDirection, GetDefaultAimPoint());
            }
        }

        public Transform GetRootTransform()
        {
            return rootTransform;
        }

        public void OnFiringButtonPressed()
        {
            weapons[weaponIndex].OnTriggerHold();
        }

        public void OnFiringButtonReleased()
        {
            weapons[weaponIndex].OnTriggerRelease();
        }

        public void OnAimAssist(bool locked, Targetable target)
        {
            this.assisting = locked;
            assistTarget = target;
        }

        public void ClearAimAssist()
        {
            assisting = false;
            assistTarget = null;
        }

        //public void AddTarget(Targetable target)
        //{
        //    if (lockTargets.Count < weaponData[weaponIndex].maxTargets && !lockTargets.Contains(target))
        //    {
        //        lockTargets.Add(target);
        //    }

        //    assisting = lockTargets.Count > 0;
        //}

        //public void ResetTargets()
        //{
        //    lockTargets.Clear();
        //    assisting = false;
        //}

        public void SetAimTransform(Transform point)
        {
            aimTransform = point;
        }

        public Vector3 GetFiringDirection(Transform firingPoint, WeaponData data)
        {
            if (assisting)
            {
                (Vector3 targetPos, Vector3 targetVelocity) = assistTarget.GetPositionInfo();

                Vector3 dir = WeaponUtilities.FirstOrderIntercept(firingPoint.position, Vector3.zero, data.projectileSpeed, targetPos, targetVelocity);
                dir = Vector3.RotateTowards(firingPoint.forward, dir, assistRads, 0f);
                return dir;
            }

            return firingPoint.forward;
        }

        public WeaponData GetCurrentWeaponInfo()
        {
            return weaponData[weaponIndex];
        }

        public Vector3 GetDefaultAimPoint()
        {
            if (aimTransform == null)
            {
                return Vector3.zero;
            }
            return aimTransform.position;
        }
    }
}
