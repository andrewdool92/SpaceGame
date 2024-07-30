using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Shard : MonoBehaviour
{
    private Collider shardCollider;
    public Rigidbody rb;

    private Transform defaultParent;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    void Awake()
    {
        shardCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        defaultParent = transform.parent;
        defaultPosition = transform.localPosition;
        defaultRotation = transform.localRotation;
    }

    public void ResetPosition()
    {
        transform.SetParent(defaultParent);
        transform.localPosition = defaultPosition;
        transform.localRotation = defaultRotation;
    }

    public void AddForces(Vector3 explosionPoint, float explosionForce, float explosionRadius, Vector3 collisionPoint, float projectileForce, float projectileRadius)
    {
        rb.AddExplosionForce(explosionForce, explosionPoint, explosionRadius);
        rb.AddExplosionForce(projectileForce, collisionPoint, projectileRadius);
    }

    public void BreakOff(Vector3 explosionForce, Vector3 collisionForce, float torqueModifier)
    {
        transform.SetParent(null);

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        rb.detectCollisions = true;
        rb.useGravity = false;
        rb.angularDrag = 0.05f;
        rb.drag = 1;

        shardCollider.enabled = true;

        rb.AddForce(explosionForce + collisionForce);
        rb.AddTorque(Random.insideUnitSphere * torqueModifier);
    }
}
