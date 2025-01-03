using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class BaseProjectile : MonoBehaviour
    {
        public GameObject particle;
        protected IWeapon launcher;
        protected WeaponData weaponData;
        protected Transform rootTransform;

        protected float range;
        protected float speed;
        protected bool active = false;
        protected Vector3 lastPos;
        protected Ray checkRay = new Ray();

        public virtual void SetLauncher(IWeapon launcher, WeaponData weaponData)
        {
            this.launcher = launcher;
            this.weaponData = weaponData;

            range = weaponData.projectileRange;
            speed = weaponData.projectileSpeed;
            rootTransform = launcher.GetRootTransform();
        }

        public virtual void Launch(Vector3 firingPoint, Vector3 direction, Vector3 point)
        {
            transform.position = firingPoint;
            transform.forward = ApplyError(direction);
            lastPos = transform.position;
            range = weaponData.projectileRange;

            if (!active)
            {
                Enable();
            }
        }

        public virtual void Launch(Vector3 firingPoint, Vector3 direction, Targetable target)
        {
            Launch(firingPoint, direction, Vector3.zero);
        }

        protected virtual void Move(float deltaTime)
        {
            float moveDist = Mathf.Min(speed * deltaTime, range);
            checkRay.origin = transform.position;
            checkRay.direction = transform.forward;

            if (Physics.Raycast(checkRay, out RaycastHit hit, moveDist))
            {
                CheckCollision(hit);
            }

            CheckLifetime(moveDist);
            lastPos = transform.position;
            transform.position += transform.forward * moveDist;
        }

        protected virtual void CheckLifetime(float moveDist)
        {
            range -= moveDist;
            if (range <= 0f)
            {
                EndOfLife();
            }
        }

        protected void CheckCollision(RaycastHit hit)
        {
            if (hit.transform.IsChildOf(rootTransform)) return;

            HandleCollision(hit);
        }

        protected virtual void HandleCollision(RaycastHit hit)
        {
            if (weaponData.blastRadius != 0f)
            {
                Detonate();
            }
            else
            {
                Vector3 velocity = transform.forward * speed;
                DamageInstance damageInstance = weaponData.GetDamageInstance(hit.point, velocity);

                if (hit.transform.TryGetComponent<IDamageable>(out IDamageable target))
                {
                    target.Damage(damageInstance);
                }
                else if (hit.transform.TryGetComponent<Rigidbody>(out Rigidbody body))
                {
                    body.AddForceAtPosition(hit.point, velocity.normalized * damageInstance.blastPower);
                }
            }

            launcher.PlayHitEffect(hit.point);
            launcher.ApplyBlastMark(hit);
            EndOfLife();
        }

        public virtual void Enable()
        {
            active = true;
            particle.SetActive(true);
            WeaponManager.ProjectileUpdate += Move;
        }

        public virtual void Disable()
        {
            active = false;
            particle.SetActive(false);
            WeaponManager.ProjectileUpdate -= Move;
        }

        protected virtual void EndOfLife()
        {
            if (weaponData.blastRadius != 0f)
            {
                Detonate();
            }

            Disable();
        }

        protected void Detonate()
        {
            DamageInstance damage = weaponData.GetDamageInstance(transform.position, transform.forward * speed);
            IDamageable target;
            Rigidbody rb;

            Collider[] targets = Physics.OverlapSphere(transform.position, damage.blastRadius);
            foreach (Collider collider in targets)
            {
                Debug.Log(collider.gameObject.name);
                if (collider.gameObject.TryGetComponent<IDamageable>(out target))
                {
                    Debug.Log($"Dealing explosive damage to {target}");
                    target.Damage(damage);
                }
                else if (collider.gameObject.TryGetComponent<Rigidbody>(out rb))
                {
                    rb.AddExplosionForce(damage.blastPower, damage.hitPoint, damage.blastRadius);
                }
            }

            launcher.PlayHitEffect(transform.position);
            Disable();
        }

        private Vector3 ApplyError(Vector3 dir)
        {
            Vector2 errorVector = Random.insideUnitCircle * weaponData.maxError;
            Quaternion error = Quaternion.Euler(errorVector.x, errorVector.y, 0);

            return error * dir;
        }
    }
}