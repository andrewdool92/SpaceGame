using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHitEffect : MonoBehaviour
{
    public ParticleSystem sparks;

    public void PlayAtLocation(Vector3 position)
    {
        transform.position = position;
        sparks.Play();
    }
}
