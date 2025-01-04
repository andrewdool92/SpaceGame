using UnityEngine;
using SpaceGame.Utils;

namespace Weapons
{
    public class GuidedProjectile : BaseProjectile
    {
        private Targetable target;
        private bool targetSelected = false;
        private bool targetLocked = false;
        private Vector3 unlockedPoint = Vector3.zero;

        [SerializeField] private TrailRenderer trail;

        public override void Launch(Vector3 firingPoint, Vector3 direction, Vector3 point)
        {
            trail.emitting = false;

            this.targetSelected = false;
            this.targetLocked = false;
            unlockedPoint = point;
            base.Launch(firingPoint, direction, point);

            trail.Clear();
            trail.emitting = true;
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

            Vector3 targetPosition = targetSelected ? WeaponUtilities.FirstOrderIntercept(transform.position, Vector3.zero, speed, target.GetPosition(), target.GetVelocity()) : unlockedPoint;
            Vector3 targetDir = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.RotateTowards(transform.forward, targetDir, Mathf.Deg2Rad * turnSpeed * deltaTime, 0f);

            if (!targetLocked)
            {
                targetLocked = Vector3.Angle(transform.forward, targetDir) < 1f;
            }
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
        }
    }
}