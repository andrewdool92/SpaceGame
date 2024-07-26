using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ThrusterController))]
public class ShipController : MonoBehaviour
{
    public float pitchTorque, yawTorque, rollTorque, verticalThrust, strafeThrust, forwardThrust, reverseThrust, superBoostThrust;
    public float superBoostPenalty;

    private Vector2 mousePosition;
    private Vector2 screenCentre;
    private Vector2 aimDirection = Vector2.zero;

    private Rigidbody rb;
    private ThrusterController thrusters;

    private float pitchValue, yawValue, rollValue = 0;
    private float strafeValue, verticalValue = 0;
    private bool boosting, reversing, superBoosting = false;
    private float steerPenalty = 1;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        thrusters = GetComponent<ThrusterController>();
        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (pitchValue != 0) rb.AddRelativeTorque(-Vector3.right * pitchValue * pitchTorque * steerPenalty);
        if (yawValue != 0) rb.AddRelativeTorque(Vector3.up * yawValue * yawTorque * steerPenalty);
        if (rollValue != 0) rb.AddRelativeTorque(Vector3.forward * rollValue * rollTorque);

        if (strafeValue != 0) rb.AddRelativeForce(Vector3.right * strafeValue * strafeThrust);
        if (verticalValue != 0) rb.AddRelativeForce(Vector3.up * verticalValue * verticalThrust);

        if (superBoosting) rb.AddRelativeForce(Vector3.forward * superBoostThrust);
        else if (boosting) rb.AddRelativeForce(Vector3.forward * forwardThrust);
        else if (reversing) rb.AddRelativeForce(Vector3.back * reverseThrust);

    }

    private void HandleThrusters()
    {
        thrusters.SetForwardThrusters(boosting);
        thrusters.SetReverseThrusters(reversing);

        thrusters.SetStrafe(strafeValue);
        thrusters.SetUpDown(verticalValue);
        thrusters.SetRoll(rollValue);
    }

    public void HandleAim(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        aimDirection =  mousePosition - screenCentre;

        pitchValue = (Mathf.Abs(aimDirection.y) > 100) ? Mathf.Clamp(aimDirection.y / 100, -3, 3) : 0;
        yawValue = (Mathf.Abs(aimDirection.x) > 100) ? Mathf.Sign(aimDirection.x) : 0;
    }

    public void HandleStrafe(InputAction.CallbackContext context)
    {
        strafeValue = context.ReadValue<float>();
        thrusters.SetStrafe(strafeValue);
    }

    public void HandleUpDown(InputAction.CallbackContext context)
    {
        verticalValue = context.ReadValue<float>();
        thrusters.SetUpDown(verticalValue);
    }

    public void HandleBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
        thrusters.SetForwardThrusters(boosting);

        if (!boosting)
        {
            superBoosting = false;
            thrusters.SetSuperBoost(false);
            steerPenalty = 1;
        }
    }

    public void HandleReverse(InputAction.CallbackContext context)
    {
        reversing = context.performed;
        thrusters.SetReverseThrusters(reversing);
    }

    public void HandleRoll(InputAction.CallbackContext context)
    {
        rollValue = context.ReadValue<float>();
        thrusters.SetRoll(rollValue);
    }

    public void HandleSuperBoost(InputAction.CallbackContext context)
    {
        if (boosting && context.performed)
        {
            superBoosting = true;
            thrusters.SetSuperBoost(true);
            steerPenalty = superBoostPenalty;
        }
    }
}
