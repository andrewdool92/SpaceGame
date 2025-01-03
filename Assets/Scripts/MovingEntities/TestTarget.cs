using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestTarget : MonoBehaviour
{
    public float velocity;
    public float turnSpeed;
    public Vector3 rotationAxis = Vector3.up;

    public bool patrol = false;
    public float redirectTime = 0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (patrol)
        {
            StartCoroutine(Patrol());
        }
    }

    void FixedUpdate()
    {
        transform.rotation *= Quaternion.AngleAxis(turnSpeed * Time.deltaTime, rotationAxis);
        rb.velocity = transform.forward * velocity;
    }

    private IEnumerator Patrol()
    {
        while(true)
        {
            yield return new WaitForSeconds(redirectTime);
            transform.forward = -transform.forward;
        }
    }
}
