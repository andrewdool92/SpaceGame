using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public float pitchTorque, yawTorque, rollTorque, verticalThrust, strafeThrust, forwardThrust, reverseThrust;

    public Vector2 mousePosition;
    private Vector2 screenCentre;
    private Vector2 aimDirection = Vector2.zero;

    private Rigidbody rb;

    private float pitchValue, yawValue, rollValue = 0;
    private float strafeValue, verticalValue = 0;
    private bool boosting, reversing = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (pitchValue != 0) rb.AddRelativeTorque(-Vector3.right * pitchValue * pitchTorque);
        if (yawValue != 0) rb.AddRelativeTorque(Vector3.up * yawValue * yawTorque);
        if (rollValue != 0) rb.AddRelativeTorque(Vector3.forward * rollValue * rollTorque);

        if (strafeValue != 0) rb.AddRelativeForce(Vector3.right * strafeValue * strafeThrust);
        if (verticalValue != 0) rb.AddRelativeForce(Vector3.up * verticalValue * verticalThrust);

        if (boosting) rb.AddRelativeForce(Vector3.forward * forwardThrust);
        else if (reversing) rb.AddRelativeForce(Vector3.back * reverseThrust);
    }

    public void HandleAim(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        aimDirection =  mousePosition - screenCentre;

        pitchValue = (Mathf.Abs(aimDirection.y) > 100) ? Mathf.Sign(aimDirection.y) : 0;
        yawValue = (Mathf.Abs(aimDirection.x) > 100) ? Mathf.Sign(aimDirection.x) : 0;
    }

    public void HandleStrafe(InputAction.CallbackContext context)
    {
        strafeValue = context.ReadValue<float>();
    }

    public void HandleUpDown(InputAction.CallbackContext context)
    {
        verticalValue = context.ReadValue<float>();
    }

    public void HandleBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }

    public void HandleReverse(InputAction.CallbackContext context)
    {
        reversing = context.performed;
    }

    public void HandleRoll(InputAction.CallbackContext context)
    {
        rollValue = context.ReadValue<float>();
    }
}
