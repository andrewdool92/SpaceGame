using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Thruster : MonoBehaviour
{
    public TrailRenderer trail;
    public MeshRenderer flare;
    public Rigidbody parentRb;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (active && Vector3.Angle(parentRb.velocity, transform.forward) < 30)
        //{
        //    trail.emitting = true;
        //}
        //else
        //{
        //    trail.emitting = false;
        //}
    }

    public void SetThrust(bool value)
    {
        flare.gameObject.SetActive(value);
    }

    public void SetTrail(bool value)
    {
        trail.emitting = value;
        //if (value)
        //{
        //    trail.Clear();
        //}
    }
}
