using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingedStructure : MonoBehaviour
{
    public Transform hinge;

    public Vector2 angleRange = new(20, 20);
    public float rotationSpeed = 5;
    public float swivleSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate(Time.deltaTime);
    }

    private void Rotate(float deltaTime)
    {
        //hinge.Rotate(new Vector3(0, rotationSpeed * deltaTime, 0));

        hinge.RotateAround(transform.position, transform.up, rotationSpeed * deltaTime);
    }
}
