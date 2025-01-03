using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HomingProjectile : Projectile
{
    public float fuelDistance;
    public float fuelAccelleration = 1.1f;
    public float fuelDrainedDecelleration = .9f;
    public float turnSpeed;

    public Rigidbody target;
    private bool targetLocked = false;

    public VisualEffect trail;
    private int powerID;

    protected override void Start()
    {
        base.Start();
        powerID = Shader.PropertyToID("ThrusterPower");
    }

    protected override void EnableProjectile()
    {
        base.EnableProjectile();
        trail.Play();
        trail.SetFloat(powerID, 2f);
    }

    protected override void DisableProjectile()
    {
        base.DisableProjectile();
        trail.Stop();
    }

    protected override void Move()
    {
        base.Move();
        Aim();
    }

    private void Aim()
    {
        if (range - fuelDistance > 0)
        {
            velocity *= fuelAccelleration;

            if (!target.gameObject.activeSelf)
            {
                targetLocked = false;
            }
            
            if (targetLocked)
            {
                Vector3 targetPosition = WeaponUtilities.FirstOrderIntercept(transform.position, Vector3.zero, velocity.magnitude, target.position, target.velocity);
                Vector3 targetDir = targetPosition - transform.position;
                transform.forward = Vector3.RotateTowards(transform.forward, targetDir, Mathf.Deg2Rad * turnSpeed, 0f);
            }
        }
        else
        {
            trail.SetFloat(powerID, (range - fuelDistance) / range);
            velocity *= fuelDrainedDecelleration;
        }
    }

    protected override void CheckLifetime(float delta)
    {
        range -= delta;
        if (range < 0f)
        {
            Detonate();
        }
    }

    public void SetTarget(Rigidbody target)
    {
        this.target = target;
        targetLocked = true;
    }

    protected override void Detonate()
    {
        DamageInstance damage = parent.GetDamageInstance(lastPosition, velocity);
        IDamageable target;
        Rigidbody rb;

        Collider[] targets = Physics.OverlapSphere(lastPosition, damage.blastRadius);
        foreach(Collider collider in targets)
        {
            if (collider.gameObject.TryGetComponent<IDamageable>(out target))
            {
                target.Damage(damage);
            }
            else if (collider.gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb.AddExplosionForce(damage.blastPower, damage.hitPoint, damage.blastRadius);
            }
        }
    }

    protected override void HandleCollisionDamage(RaycastHit hit)
    {
        Detonate();
    }
}
