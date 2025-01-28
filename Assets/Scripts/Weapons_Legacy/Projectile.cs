using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    public ParticleHitEffect explosionEffect;
    public ImpactDecal blastMark;
    protected MeshRenderer particle;
    public Vector3 velocity = Vector3.zero;

    public Transform rootTransform;

    protected Vector3 lastPosition;

    public float range = 0f;

    public IWeapon_OLD parent;

    protected bool active = false;

    public float decalMinSize = .5f;
    public float decalMaxSize = 3f;
    public float decalFadeSpeed = .1f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        particle = GetComponentInChildren<MeshRenderer>();

        blastMark.Initialize();

        DisableProjectile();
    }

    private void OnEnable()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        Move();
    }

    protected virtual void Move()
    {
        transform.position += velocity * Time.deltaTime;
        Vector3 ray = transform.position - lastPosition;
        float dist = ray.magnitude;

        if (Physics.Raycast(lastPosition, ray, out RaycastHit hit, Mathf.Min(dist, range)))
        {
            CheckCollision(hit);
        }

        CheckLifetime(dist);

        lastPosition = transform.position;
    }

    protected virtual void CheckLifetime(float delta)
    {
        range -= delta;
        if (range <= 0f)
        {
            DisableProjectile();
        }
    }

    public void Fire(Vector3 point, Vector3 velocity, float range)
    {
        active = false;
        transform.position = point;
        transform.forward = velocity;
        lastPosition = point;
        this.velocity = velocity;
        this.range = range;

        EnableProjectile();
    }

    protected virtual void EnableProjectile()
    {
        particle.gameObject.SetActive(true);
        active = true;
    }

    protected virtual void DisableProjectile()
    {
        active = false;
        particle.gameObject.SetActive(false);
    }

    protected virtual void Detonate()
    {
        parent.PlayHitEffect(lastPosition);
        DisableProjectile();
    }

    protected virtual void HandleCollisionDamage(RaycastHit hit)
    {
        DamageInstance damageInstance = parent.GetDamageInstance(hit.point, velocity);

        if (hit.transform.TryGetComponent<IDamageable>(out IDamageable target))
        {
            target.Damage(damageInstance);
        }
        else if (hit.transform.TryGetComponent<Rigidbody>(out Rigidbody body))
        {
            body.AddForceAtPosition(hit.point, velocity.normalized * damageInstance.blastPower);
        }

        Detonate();
    }

    private void CheckCollision(RaycastHit hit)
    {
        if (hit.transform.IsChildOf(rootTransform)) return;

        HandleCollisionDamage(hit);
        ApplyBlastMark(hit);
    }

    private void ApplyBlastMark(RaycastHit hit)
    {
        float decalSize = Random.Range(decalMinSize, decalMaxSize);

        blastMark.Apply(decalSize, hit);
    }

    public void SetParent(IWeapon_OLD parent)
    {
        this.parent = parent;
        rootTransform = parent.GetRootTransform();
    }
}
