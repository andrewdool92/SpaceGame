using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldController : MonoBehaviour
{
    public VisualEffect shieldHit;
    public VisualEffect shieldShatter;
    public Collider shieldCollider;
    public List<Collider> bodyColliders;

    public float maxHealth;
    [SerializeField] private float currentHealth;

    public float rechargeDelay;
    public float rechargeSpeed;
    [SerializeField] private float rechargeTimer;

    public bool alive;

    private Destructible hull;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        alive = true;

        TryGetComponent<Destructible>(out hull);
    }

    private void Update()
    {
        if (!alive) return;

        if (rechargeTimer <= 0 && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Clamp(currentHealth + rechargeSpeed * Time.deltaTime, 0, maxHealth);
            shieldCollider.enabled = true;
            SetBodyColliders(false);
            return;
        }

        rechargeTimer -= Time.deltaTime;
    }

    public float Damage(float value, Vector3 direction)
    {
        rechargeTimer = rechargeDelay;

        if (currentHealth  <= 0f)
        {
            return value;
        }

        float diff = value - currentHealth;
        currentHealth -= value;

        if (currentHealth <= 0)
        {
            shieldHit.Reinit();
            shieldShatter.SetVector3("CollisionDirection", direction);
            shieldShatter.Play();
            shieldCollider.enabled = false;
            SetBodyColliders(true);
            return diff;
        }
        else
        {
            shieldHit.SetFloat("HealthPercentage", currentHealth / maxHealth);
            shieldHit.Play();
            return 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamageInstance hit = new DamageInstance(1f, 0f, 0f, collision.GetContact(0).point, collision.relativeVelocity);
        float remaining = Damage(1f, collision.relativeVelocity.normalized);

        if (remaining > 0f)
        {
            hit.damage = remaining;
            hull?.Damage(hit);
        }
    }

    public bool IsBroken()
    {
        return currentHealth <= 0f;
    }

    private void SetBodyColliders(bool value)
    {
        foreach(Collider collider in bodyColliders)
        {
            collider.enabled = value;
        }
    }
}
