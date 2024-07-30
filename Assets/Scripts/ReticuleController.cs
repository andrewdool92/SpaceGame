using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticuleController : MonoBehaviour
{
    public Transform reticule;
    public float maxDistance;
    public float reticuleRotationSpeed;

    private Vector3 reticuleRotationEuler;

    private void Start()
    {
        //reticuleRotationEuler = new Vector3(0, 0, reticuleRotationSpeed);
    }

    private void Update()
    {
        //reticule.Rotate(reticuleRotationEuler);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            reticule.position = hit.point;
            reticule.rotation = Quaternion.LookRotation(-hit.normal);
            //Debug.DrawRay(hit.point, hit.normal * 20, Color.red, .5f);
        }
        else
        {
            reticule.position = transform.position + transform.forward * maxDistance;
            reticule.rotation = transform.rotation;
        }
    }
}
