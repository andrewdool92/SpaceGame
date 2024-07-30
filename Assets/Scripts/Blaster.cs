using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Blaster : MonoBehaviour
{
    public ParticleHitEffect explosion;

    public ParticleSystem blasterParticles;
    public List<ParticleCollisionEvent> collisionEvents = new();

    public DecalProjector blastMark;
    public int totalDecals = 10;
    private Queue<DecalProjector> decalPool = new();

    public float decalMinSize = .5f;
    public float decalMaxSize = 3f;
    public float decalFadeSpeed = .1f;

    public float damage = 1f;
    public float blastPower = 1f;
    public float blastRadius = 5f;

    // Start is called before the first frame update
    void Start()
    {
        blasterParticles = GetComponent<ParticleSystem>();

        GenerateDecalPool();
    }


    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = blasterParticles.GetCollisionEvents(other, collisionEvents);
        float remainingDamage = damage;

        foreach (ParticleCollisionEvent e in collisionEvents)
        {
            explosion.PlayAtLocation(e.intersection);

            if (other.TryGetComponent<ShieldController>(out ShieldController shield))
            {
                remainingDamage = shield.Damage(damage, e.velocity.normalized);

                if (remainingDamage <= 0) return;
            }
            if (other.TryGetComponent<Destructible>(out Destructible hull))
            {
                hull.Damage(remainingDamage, blastPower, blastRadius, e.intersection, e.velocity);
            }
            else if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForceAtPosition(e.intersection, e.velocity.normalized * blastPower);
            }
            ApplyBlastMark(e, other);
        }
    }

    private void GenerateDecalPool()
    {
        for (int i = 0; i < totalDecals; i++)
        {
            DecalProjector mark = Instantiate(blastMark, transform);
            mark.gameObject.SetActive(true);
            mark.fadeFactor = 0f;
            decalPool.Enqueue(mark);
            StartCoroutine(FadeBlastMark(mark));
        }
    }

    private void ApplyBlastMark(ParticleCollisionEvent e, GameObject other)
    {
        DecalProjector mark = decalPool.Dequeue();
        decalPool.Enqueue(mark);
        float decalSize = Random.Range(decalMinSize, decalMaxSize);

        mark.size = new Vector3(decalSize, decalSize, decalSize);

        mark.transform.position = e.intersection;
        mark.transform.forward = -e.normal;
        mark.transform.SetParent(other.transform);
        mark.gameObject.SetActive(true);
        mark.fadeFactor = 1f;
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
