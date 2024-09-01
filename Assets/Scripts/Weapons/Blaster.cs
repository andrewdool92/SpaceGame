using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Blaster : MonoBehaviour, IWeapon
{
    public Transform rootTransform;
    public ParticleHitEffect explosion;
    public ParticleSystem muzzleFlare;

    public Projectile projectileTemplate;
    public int maxProjectiles = 20;
    public Queue<Projectile> projectilePool;

    public float firingSpeed = 1f;

    public float damage = 1f;
    public float blastPower = 1f;
    public float blastRadius = 5f;

    public float projectileRange = 300f;
    public float projectileSpeed = 50f;
    public float maxError = 1.5f;

    public List<WeaponHardpoint> weaponHardpoints;

    private int hardpointIndex = 0;
    private IEnumerator firingSequence;

    public AmmoSystem ammunition;
    public bool infiniteAmmo = true;
    private delegate void OnAmmoEmpty();
    private event OnAmmoEmpty ammoEmpty;

    private void Start()
    {
        if (rootTransform == null) rootTransform = transform;
    }

    public DamageInstance GetDamageInstance(Vector3 point, Vector3 velocity)
    {
        return new DamageInstance(damage, blastPower, blastRadius, point, velocity);
    }

    public void Initialize()
    {
        projectilePool = new();

        for (int i = 0; i < maxProjectiles; i++)
        {
            Projectile projectile = Instantiate(projectileTemplate);
            projectile.SetParent(this);
            projectile.gameObject.SetActive(true);

            projectilePool.Enqueue(projectile);
        }
    }

    public void StartFire()
    {
        firingSequence = FireSequence();
        StartCoroutine(firingSequence);
    }

    public void ReleaseFire() 
    {
        StopCoroutine(firingSequence);
    }

    public IEnumerator FireSequence()
    {
        Vector2 errorVector;
        Quaternion error;
        Vector3 firingDirection;
        Projectile proj;
        Transform firingPoint;

        while (true)
        {
            firingPoint = weaponHardpoints[hardpointIndex].transform;

            errorVector = Random.insideUnitCircle * maxError;
            error = Quaternion.Euler(errorVector.x, errorVector.y, 0);
            firingDirection = error * transform.forward;

            proj = projectilePool.Dequeue();
            proj.Fire(firingPoint.position, firingDirection * projectileSpeed, projectileRange);
            projectilePool.Enqueue(proj);

            muzzleFlare.transform.position = firingPoint.position;
            muzzleFlare.Play();
            weaponHardpoints[hardpointIndex].PlayAnimation();

            hardpointIndex = (hardpointIndex + 1) % weaponHardpoints.Count;
            SpendAmmo();

            yield return new WaitForSeconds(firingSpeed);
        }
    }

    public void PlayHitEffect(Vector3 point)
    {
        explosion.PlayAtLocation(point);
    }

    public void SetHardpoints(List<WeaponHardpoint> hardpoints)
    {
        weaponHardpoints = hardpoints;
    }

    public Transform GetRootTransform()
    {
        return rootTransform;
    }

    public void AddEventListener(IWeaponListener listener)
    {
        ammoEmpty += listener.OnAmmoEmpty;
    }
    public void RemoveEventListener(IWeaponListener listener)
    {
        ammoEmpty -= listener.OnAmmoEmpty;
    }

    private void SpendAmmo()
    {
        if (infiniteAmmo) return;

        bool ammoLeft = ammunition.SpendAmmo(1);
        if (!ammoLeft) ammoEmpty?.Invoke();
    }





    // below is being phased out

    public void Fire(Transform point, Vector3 currentVelocity)
    {
        Vector2 errorVector = Random.insideUnitCircle * maxError;
        Quaternion error = Quaternion.Euler(errorVector.x, errorVector.y, 0);

        Vector3 firingDirection = error * point.forward;

        Projectile proj = projectilePool.Dequeue();
        proj.Fire(point.position, firingDirection * projectileSpeed, projectileRange);
        projectilePool.Enqueue(proj);
    }

    public void Fire(Transform point)
    {
        Fire(point, Vector3.zero);
    }

    public Queue<Projectile> GenerateProjectilPool()
    {
        Queue<Projectile> pool = new();

        for (int i = 0; i < maxProjectiles; i++)
        {
            Projectile projectile = Instantiate(projectileTemplate);
            projectile.SetParent(this);
            projectile.gameObject.SetActive(true);

            pool.Enqueue(projectile);
        }

        projectileTemplate.gameObject.SetActive(false);
        projectilePool = pool;
        return pool;
    }

}
