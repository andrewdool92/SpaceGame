using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Thruster : MonoBehaviour
{
    public TrailRenderer trail;
    public MeshRenderer flare;
    public MovementController movement;

    public VisualEffect thrusterFX;
    public Rigidbody shipRigidBody;
    private int velocityID, boostingID, powerID;

    public Vector3 thrustActivationAngle = Vector3.forward;
    public Vector3 rotationActivationAngle = Vector3.zero;
    private bool rotationActivation = false;

    public bool boosters = false;

    private bool movementActive = false, rotationActive = false;

    private void Awake()
    {
        rotationActivation = rotationActivationAngle == Vector3.zero;
        velocityID = Shader.PropertyToID("ShipVelocity");
        boostingID = Shader.PropertyToID("Boosting");
        powerID = Shader.PropertyToID("ThrusterPower");
    }

    private void FixedUpdate()
    {
        if (boosters)
        {
            thrusterFX.SetVector3(velocityID, shipRigidBody.velocity);
        }
    }

    private void OnEnable()
    {
        movement.MovementUpdate += OnMovementUpdate;
        if (rotationActivation) movement.RotationUpdate += OnRotationUpdate;
        if (boosters)
        {
            movement.OnBoost += OnBoostUpdate;
            trail.gameObject.SetActive(true);
            trail.emitting = false;
        }
    }

    private void OnDisable()
    {
        movement.MovementUpdate -= OnMovementUpdate;
        if (rotationActivation) movement.RotationUpdate -= OnRotationUpdate;
        if (boosters) movement.OnBoost -= OnBoostUpdate;
    }

    private void OnMovementUpdate(Vector3 direction)
    {
        movementActive = Vector3.Angle(thrustActivationAngle, direction) < 85;
        SetThrust(movementActive || rotationActive);
    }

    private void OnRotationUpdate(Vector3 rotation)
    {
        rotationActive = Vector3.Angle(rotationActivationAngle, rotation) < 85;
        SetThrust(movementActive || rotationActive);
    }

    private void OnBoostUpdate(bool boosting)
    {
        SetTrail(boosting);
    }


    public void SetThrust(bool value)
    {
        flare.gameObject.SetActive(value);
    }

    public void SetTrail(bool value)
    {
        trail.emitting = value;
    }
}
