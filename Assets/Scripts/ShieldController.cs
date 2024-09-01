using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;

public class ShieldController : MonoBehaviour, IDamageable
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

    public void Damage(DamageInstance hit)
    {
        rechargeTimer = rechargeDelay;

        if (currentHealth <= 0f)
        {
            hull?.Damage(hit);
            return;
        }

        float diff = hit.damage - currentHealth;
        currentHealth -= hit.damage;
        hit.damage = diff;

        if (currentHealth <= 0)
        {
            Break(hit);
        }
        else
        {
            shieldHit.SetFloat("HealthPercentage", currentHealth / maxHealth);
            shieldHit.Play();
        }
    }

    public void Break(DamageInstance hit)
    {
        shieldHit.Reinit();
        shieldShatter.SetVector3("CollisionDirection", hit.hitVelocity);
        shieldShatter.Play();
        shieldCollider.enabled = false;
        SetBodyColliders(true);
        hull?.Damage(hit);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamageInstance hit = new DamageInstance(1f, 0f, 0f, collision.GetContact(0).point, collision.relativeVelocity);
        Damage(hit);
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
