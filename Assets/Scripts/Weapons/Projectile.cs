using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    public ParticleHitEffect explosionEffect;
    private DecalProjector impactMark;
    private LineRenderer trail;
    private MeshRenderer particle;
    public Vector3 velocity = Vector3.zero;

    public Transform rootTransform;

    private Vector3 lastPosition;

    public float range = 0f;

    public float decalMinSize = .5f;
    public float decalMaxSize = 3f;
    public float decalFadeSpeed = .1f;

    public float damage = 1f;
    public float blastPower = 1f;
    public float blastRadius = 5f;

    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        impactMark = GetComponentInChildren<DecalProjector>();
        trail = GetComponentInChildren<LineRenderer>();
        particle = GetComponentInChildren<MeshRenderer>();

        StartCoroutine(FadeBlastMark(impactMark));
        impactMark.fadeFactor = 0;

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
        if (hit.transform.root == rootTransform) return;

        explosionEffect.PlayAtLocation(lastPosition);
        float remainingDamage = damage;

        if (hit.transform.TryGetComponent<ShieldController>(out ShieldController shield))
        {
            remainingDamage = shield.Damage(damage, velocity.normalized);

            if (remainingDamage <= 0f) return;
        }
        if (hit.transform.TryGetComponent<Destructible>(out Destructible hull))
        {
            hull.Damage(remainingDamage, blastPower, blastRadius, hit.point, velocity);
        }
        else if (hit.transform.TryGetComponent<Rigidbody>(out Rigidbody body))
        {
            body.AddForceAtPosition(hit.point, velocity.normalized * blastPower);
        }

        ApplyBlastMark(hit);
        DisableProjectile();
    }

    private void ApplyBlastMark(RaycastHit hit)
    {
        float decalSize = Random.Range(decalMinSize, decalMaxSize);

        impactMark.size = new Vector3(decalSize, decalSize, decalSize);

        impactMark.transform.position = hit.point;
        impactMark.transform.forward = -hit.normal;
        impactMark.transform.SetParent(hit.transform);
        impactMark.gameObject.SetActive(true);
        impactMark.fadeFactor = 1f;
    }

    private IEnumerator FadeBlastMark(DecalProjector mark)
    {
        while (true)
        {
            if (mark.fadeFactor > 0)
            {
                mark.fadeFactor -= decalFadeSpeed;
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
