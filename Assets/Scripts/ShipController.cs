using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
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

    public float pitchValue, yawValue, rollValue = 0;
    public float strafeValue, verticalValue = 0;
    public bool boosting, reversing, superBoosting = false;
    private float steerPenalty = 1;

    public CinemachineVirtualCamera playerCam;
    public Destructible hull;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        thrusters = GetComponent<ThrusterController>();
        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void Start()
    {
        hull.onDestruction += onShipDestroyed;
    }

    private void OnDestroy()
    {
        hull.onDestruction -= onShipDestroyed;
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

    public void UpdateThrusters()
    {
        thrusters.SetForwardThrusters(boosting || superBoosting);
        //thrusters.SetReverseThrusters(reversing);

        //thrusters.SetStrafe(strafeValue);
        //thrusters.SetUpDown(verticalValue);
        //thrusters.SetRoll(rollValue);

        thrusters.SetSuperBoost(superBoosting);
        if (superBoosting)
        {
            steerPenalty = superBoostPenalty;
        }
        else
        {
            steerPenalty = 1;
        }
    }

    public void HandleAim(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        aimDirection =  mousePosition - screenCentre;

        if (aimDirection.magnitude < 50)
        {
            pitchValue = 0;
            yawValue = 0;
            return;
        }

        pitchValue = Mathf.Clamp(aimDirection.y / 200, -3, 3);
        yawValue = Mathf.Clamp(aimDirection.x / 200, -3, 3);

        UpdateThrusters();
    }

    public void HandleStrafe(InputAction.CallbackContext context)
    {
        strafeValue = context.ReadValue<float>();

        UpdateThrusters();
    }

    public void HandleUpDown(InputAction.CallbackContext context)
    {
        verticalValue = context.ReadValue<float>();

        UpdateThrusters();
    }

    public void HandleBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;

        UpdateThrusters();
    }

    public void HandleReverse(InputAction.CallbackContext context)
    {
        reversing = context.performed;
    }

    public void HandleRoll(InputAction.CallbackContext context)
    {
        rollValue = context.ReadValue<float>();

        UpdateThrusters();
    }

    public void HandleSuperBoost(InputAction.CallbackContext context)
    {
        superBoosting = context.performed;

        UpdateThrusters();
    }

    private void onShipDestroyed()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        thrusters.thrusterParent.SetActive(false);

        if (TryGetComponent<PlayerInput>(out PlayerInput input))
        {
            input.enabled = false;
        }
    }
}
