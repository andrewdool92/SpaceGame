using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Destructible : MonoBehaviour, IDamageable
{
    public float maxHealth;
    [SerializeField] private float currentHealth;

    public Vector2 hitPowerBounds = new(0, 1);

    public List<Shard> shards;
    public GameObject staticRemains;
    public float explosionRadius = 10;
    public Vector2 explosionForceRange = new(1000, 2000);
    public float shardTorqueModifier = 400f;

    public GameObject baseModel;
    private Rigidbody rb;

    public ShieldController shield;
    private bool broken = false;

    public float mainExplosionDelay = 0f;
    public VisualEffect explosionFX;
    public VisualEffect windupExplosionFX;

    public Transform explosionOriginPoint;
    public float explosionOriginVariance = 0f;

    public delegate void OnDestruction();
    public event OnDestruction onDestruction;

    public Transform lockPoint;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        TryGetComponent<ShieldController>(out shield);

        if (!lockPoint) lockPoint = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(DamageInstance hit)
    {
        currentHealth = Mathf.Clamp(currentHealth - hit.damage, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Shatter(hit.blastRadius, hit.blastPower, hit.hitPoint, hit.hitVelocity.normalized);
        }
    }

    public void Shatter(float hitImpactRadius, float hitForce, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (broken) return;
        broken = true;

        if (shield)
        {
            shield.alive = false;
        }

        if (windupExplosionFX)
        {
            windupExplosionFX.SetFloat("initialExplosionsTime", mainExplosionDelay);
            windupExplosionFX.Play();
        }

        StartCoroutine(DelayedExplosion(hitImpactRadius, hitForce, hitPoint, hitDirection));
    }

    public IEnumerator DelayedExplosion(float hitImpactRadius, float hitForce, Vector3 hitPoint, Vector3 hitDirection)
    {
        yield return new WaitForSeconds(mainExplosionDelay);

        explosionFX.SetVector3("RigidbodyVelocity", rb.velocity);
        explosionFX.Play();

        baseModel.SetActive(false);
        foreach (Shard shard in shards)
        {
            shard.gameObject.SetActive(true);
            shard.ResetPosition();

            Vector3 explosionPoint = explosionOriginPoint.position + Random.insideUnitSphere * explosionOriginVariance;

            shard.rb.AddExplosionForce(Random.Range(explosionForceRange.x, explosionForceRange.y), explosionPoint, explosionRadius);
        }

        if (staticRemains) staticRemains.SetActive(true);

        onDestruction?.Invoke();
    }
}
