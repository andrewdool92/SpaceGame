using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public float pitchTorque, yawTorque, rollTorque, verticalThrust, strafeThrust, forwardThrust, reverseThrust, boostMultiplier, brakeDrag;
    public float boostPenalty;

    public float maxVelocity = 5f, maxBoostVelocity = 20f;
    private float currentSpeedCap;

    private Vector3 movementValues = Vector3.zero;
    private Vector3 rotationValues = Vector3.zero;

    private Rigidbody rb;

    [SerializeField]
    private bool boosting = false;
    private float steerPenalty = 1;
    private float boostMod = 1;
    private float storedForwardValue = 0f;

    public delegate void ThrusterUpdate(Vector3 value);
    public event ThrusterUpdate MovementUpdate;
    public event ThrusterUpdate RotationUpdate;

    public delegate void BoostUpdate(bool value);
    public event BoostUpdate OnBoost;

    public List<VisualEffect> forwardThrusters = new List<VisualEffect>();
    private int velocityID, boostingID, powerID;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        velocityID = Shader.PropertyToID("ShipVelocity");
        boostingID = Shader.PropertyToID("Boosting");
        powerID = Shader.PropertyToID("ThrusterPower");

        currentSpeedCap = maxVelocity;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.AddRelativeTorque(rotationValues, ForceMode.Acceleration);

        foreach (VisualEffect thruster in forwardThrusters)
        {
            thruster.SetVector3(velocityID, rb.velocity);
        }

        if (boosting)
        {
            ApplyBoostForce();
        }
        else
        {
            rb.AddRelativeForce(movementValues, ForceMode.Acceleration);
        }

        if (rb.velocity.sqrMagnitude > currentSpeedCap * currentSpeedCap)
        {
            rb.AddForce(-rb.velocity.normalized * (rb.velocity.magnitude - currentSpeedCap), ForceMode.Acceleration);
        }
    }

    private void ApplyBoostForce()
    {
        Vector3 boostForce = movementValues;
        Vector3 localVelocity = Quaternion.Inverse(transform.rotation) * rb.velocity;

        boostForce.y = -localVelocity.y;
        boostForce.x = -localVelocity.x;
        if (localVelocity.z < 0) boostForce.z *= 10;

        rb.AddRelativeForce(boostForce, ForceMode.Acceleration);
    }

    public void SetForward(float value)
    {
        if (boosting)
        {
            storedForwardValue = value;
            movementValues.z = forwardThrust * boostMod;
            value = 2f;
        }
        else
        {
            movementValues.z = (value > 0 ? value * forwardThrust : value * reverseThrust);
        }

        foreach(VisualEffect thruster in forwardThrusters)
        {
            thruster.SetFloat(powerID, value);
        }

        MovementUpdate?.Invoke(movementValues);
    }

    public void SetStrafe(float value)
    {
        movementValues.x = value * strafeThrust;
        MovementUpdate?.Invoke(movementValues);
    }

    public void SetVertical(float value)
    {
        movementValues.y = value * verticalThrust;
        MovementUpdate?.Invoke(movementValues);
    }

    public void SetPitch(float value)
    {
        rotationValues.x = value * pitchTorque * steerPenalty;
        RotationUpdate?.Invoke(rotationValues);
    }

    public void SetYaw(float value)
    {
        rotationValues.y = value * yawTorque * steerPenalty;
        RotationUpdate?.Invoke(rotationValues);
    }

    public void SetRoll(float value)
    {
        rotationValues.z = value * rollTorque * steerPenalty;
        RotationUpdate?.Invoke(rotationValues);
    }

    public void SetBoost(bool value)
    {
        boosting = value;

        foreach (VisualEffect thruster in forwardThrusters)
        {
            //thruster.SetBool(boostingID, value);
            thruster.SetFloat(powerID, value ? 2 : storedForwardValue);
        }

        if (boosting)
        {
            storedForwardValue = movementValues.z;
            boostMod = boostMultiplier;
            steerPenalty = boostPenalty;
            movementValues.z = forwardThrust * boostMod;

            currentSpeedCap = maxBoostVelocity;
        }
        else
        {
            boostMod = 1;
            steerPenalty = 1;
            movementValues.z = storedForwardValue;

            currentSpeedCap = maxVelocity;
        }

        OnBoost?.Invoke(value);
    }

    public void SetBrake(bool braking)
    {
        rb.drag = braking ? brakeDrag : 0;
    }

    public void SetLock(bool locked)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        movementValues = Vector3.zero;
        rotationValues = Vector3.zero;

        rb.constraints = locked ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;

        foreach(VisualEffect thruster in forwardThrusters)
        {
            thruster.Stop();
        }
    }
}
