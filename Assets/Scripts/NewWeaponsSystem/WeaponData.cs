using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapons
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "ScriptableObjects/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [Header("Projectile visual effects")]
        public ParticleHitEffect explosion;
        public ParticleHitEffect muzzleFlare;

        public BaseProjectile projectileTemplate;
        public int maxProjectiles = 20;

        public int firingDelay = 200;
        public int multitargetDelay = 50;
        public int maxTargets = 1;
        public int maxAmmo = 0;

        [Header("Damage Specs")]
        public float damage = 1f;
        public float blastPower = 1f;
        public float blastRadius = 0.3f;
        public float blastDamageFalloff = 1f;
        public float detonationRange = 0f;

        [Header("Launch Behaviour")]
        public float projectileRange = 300f;
        public float projectileSpeed = 50f;
        public float maxError = 1.5f;

        [Header("In-flight Behaviour")]
        public bool homing = false;
        public float fuelTime = 0f;
        public float fuelAccelleration = 0f;
        public float turnAccelleration = 0f;
        public float turnSpeed = 0f;
        public float preciseTurnSpeed = 0f;
        public float maxVelocity;

        [Header("Impact Visual Effects")]
        public ImpactDecal blastMark;
        public Vector2 blastMarkSizeRange;

        public HoldTriggerBehaviour triggerBehaviour;

        public enum HoldTriggerBehaviour
        {
            RAPID,
            CHARGE_SHOT,
            MULTILOCK,
            CONTINUOUS
        }

        public DamageInstance GetDamageInstance(Vector3 point, Vector3 velocity)
        {
            return new DamageInstance(damage, blastPower, blastRadius, point, velocity);
        }
    }
}