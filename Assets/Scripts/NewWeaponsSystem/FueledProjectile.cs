using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapons
{
    //[RequireComponent(typeof(Rigidbody))]
    public class FueledProjectile : BaseProjectile
    {
        private Targetable target;
        private bool targetSelected = false;
        private bool targetLocked = false;
        private Vector3 unlockedPoint = Vector3.zero;

        [SerializeField] private VisualEffect trail;
        [SerializeField] private TrailRenderer newTrail;
        private int powerID;
        //private Rigidbody rb;

        private void Start()
        {
            powerID = Shader.PropertyToID("ThrusterPower");
            //rb = GetComponent<Rigidbody>();
        }

        public override void Launch(Vector3 firingPoint, Vector3 direction, Vector3 point)
        {
            //trail.SetFloat(powerID, 1f);
            //trail.Play();
            //range = weaponData.fuelTime;

            newTrail.emitting = false;

            this.targetSelected = false;
            this.targetLocked = false;
            unlockedPoint = point;
            base.Launch(firingPoint, direction, point);

            newTrail.Clear();
            newTrail.emitting = true;

            //rb.velocity = transform.forward * weaponData.projectileSpeed;
        }

        public override void Launch(Vector3 firingPoint, Vector3 direction, Targetable target)
        {
            Launch(firingPoint, direction, unlockedPoint);

            this.target = target;
            targetSelected = true;
        }

        public void Steer(float deltaTime)
        {
            float turnSpeed = targetLocked ? weaponData.turnSpeed : weaponData.preciseTurnSpeed;

            Vector3 targetPosition = targetSelected ? WeaponUtilities.FirstOrderIntercept(transform.position, Vector3.zero, speed, target) : unlockedPoint;
            Vector3 targetDir = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.RotateTowards(transform.forward, targetDir, Mathf.Deg2Rad * turnSpeed * deltaTime, 0f);

            if (!targetLocked)
            {
                //range += speed * deltaTime;
                targetLocked = Vector3.Angle(transform.forward, targetDir) < 1f;
            }
            CheckLifetime(deltaTime);
            //else
            //{
            //    //speed += weaponData.fuelAccelleration * deltaTime;
            //    //speed = Mathf.Min(speed + weaponData.fuelAccelleration * deltaTime, weaponData.maxVelocity);
            //    CheckLifetime(deltaTime);
            //}
        }

        protected override void Move(float deltaTime)
        {
            Steer(deltaTime);
            base.Move(deltaTime);

            if (Physics.Linecast(lastPos, transform.position, out RaycastHit hit))
            {
                CheckCollision(hit);
            }

            if (targetSelected && (transform.position - target.lockPoint.position).sqrMagnitude < Mathf.Pow(weaponData.detonationRange, 2))
            {
                Detonate();
            }

            //lastPos = transform.position;
        }

        protected override void EndOfLife()
        {
            Detonate();
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

        public override void Disable()
        {
            //trail.Stop();
            base.Disable();
        }
    }
}