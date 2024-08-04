using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
    public float fuelTime;
    public float speed;
    public float turnSpeed;

    public bool targetLocked = false;
    public Rigidbody target;

    private Rigidbody rb;

    public float targetScanDelay;
    public float scanRadius;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
