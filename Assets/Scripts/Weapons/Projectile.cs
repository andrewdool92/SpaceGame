using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    public ParticleHitEffect explosionEffect;
    public ImpactDecal blastMark;
    private LineRenderer trail;
    private MeshRenderer particle;
    public Vector3 velocity = Vector3.zero;

    public Transform rootTransform;

    private Vector3 lastPosition;

    public float range = 0f;

    public IWeapon parent;

    private bool active = false;

    public float decalMinSize = .5f;
    public float decalMaxSize = 3f;
    public float decalFadeSpeed = .1f;

    // Start is called before the first frame update
    void Start()
    {
        trail = GetComponentInChildren<LineRenderer>();
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

        transform.position += velocity * Time.deltaTime;
        Vector3 ray = transform.position - lastPosition;
        float dist = ray.magnitude;

        if (Physics.Raycast(lastPosition, ray, out RaycastHit hit, Mathf.Min(dist, range)))
        {
            HandleCollision(hit);
        }

        range -= dist;
        if (range <= 0f)
        {
            DisableProjectile();
        }

        lastPosition = transform.position;
    }

    public void Fire(Vector3 point, Vector3 velocity, float range)
    {
        active = false;
        transform.position = point;
        transform.forward = velocity;
        lastPosition = point;
        this.velocity = velocity;
        this.range = range;

        particle.enabled = true;
        trail.enabled = true;
        active = true;
    }

    public void DisableProjectile()
    {
        active = false;
        trail.enabled = false;
        particle.enabled = false;
    }

    private void HandleCollision(RaycastHit hit)
    {
        //if (hit.transform.root == rootTransform) return;
        if (hit.transform.IsChildOf(rootTransform)) return;

        //explosionEffect.PlayAtLocation(lastPosition);
        parent.PlayHitEffect(lastPosition);
        DamageInstance damageInstance = parent.GetDamageInstance(hit.point, velocity);

        if (hit.transform.TryGetComponent<IDamageable>(out IDamageable target))
        {
            //target.Damage(GetHitValues(parent.damage, hit.point, velocity));
            target.Damage(damageInstance);
        }
        else if (hit.transform.TryGetComponent<Rigidbody>(out Rigidbody body))
        {
            body.AddForceAtPosition(hit.point, velocity.normalized * damageInstance.blastPower);
        }

        ApplyBlastMark(hit);
        DisableProjectile();
    }

    private void ApplyBlastMark(RaycastHit hit)
    {
        float decalSize = Random.Range(decalMinSize, decalMaxSize);

        blastMark.Apply(decalSize, hit);
    }

    public void SetParent(IWeapon parent)
    {
        this.parent = parent;
        rootTransform = parent.GetRootTransform();
    }
}
