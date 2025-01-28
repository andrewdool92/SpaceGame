using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeaponParticles : MonoBehaviour
{
    public Blaster parent;
    private ParticleSystem blasterParticles;

    private List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        blasterParticles = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = blasterParticles.GetCollisionEvents(other, collisionEvents);

        foreach (ParticleCollisionEvent e in collisionEvents)
        {
            //parent.HandleProjectileCollision(e, other);
        }
    }

    public void Fire()
    {
        blasterParticles.Play();
    }
}
