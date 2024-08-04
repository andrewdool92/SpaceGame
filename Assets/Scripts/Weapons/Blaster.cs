using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Blaster : MonoBehaviour
{
    public ParticleHitEffect explosion;

    public Projectile projectileTemplate;
    public int maxProjectiles = 20;
    public Queue<Projectile> projectilePool = new();

    public float decalMinSize = .5f;
    public float decalMaxSize = 3f;
    public float decalFadeSpeed = .1f;

    public float damage = 1f;
    public float blastPower = 1f;
    public float blastRadius = 5f;

    public float projectileRange = 300f;
    public float projectileSpeed = 50f;
    public float maxError = 1.5f;

    private void Update()
    {

    }

    public void Fire(Transform point, Vector3 direction, Vector3 currentVelocity)
    {
        Vector2 errorVector = Random.insideUnitCircle * maxError;
        Quaternion error = Quaternion.Euler(errorVector.x, errorVector.y, 0);

        Vector3 firingDirection = error * point.forward;

        Projectile proj = projectilePool.Dequeue();
        proj.Fire(point.position, firingDirection * projectileSpeed, projectileRange);
        projectilePool.Enqueue(proj);
    }

    public void Fire(Transform point, Vector3 direction)
    {
        Fire(point, direction, Vector3.zero);
    }

    public Queue<Projectile> GenerateProjectilPool()
    {
        Queue<Projectile> pool = new();

        for (int i = 0; i < maxProjectiles; i++)
        {
            Projectile projectile = Instantiate(projectileTemplate);
            projectile.rootTransform = transform.root;
            projectile.explosionEffect = explosion;
            projectile.gameObject.SetActive(true);

            pool.Enqueue(projectile);
        }

        projectileTemplate.gameObject.SetActive(false);
        projectilePool = pool;
        return pool;
    }
}
