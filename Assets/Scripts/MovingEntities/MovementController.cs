using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public float pitchTorque, yawTorque, rollTorque, verticalThrust, strafeThrust, forwardThrust, reverseThrust, boostMultiplier, brakeDrag;
    public float boostPenalty;

    public float maxVelocity = 5f;

    private Vector3 movementValues = Vector3.zero;
    private Vector3 rotationValues = Vector3.zero;

    private Rigidbody rb;

    private bool boosting = false;
    private float steerPenalty = 1;
    private float boostMod = 1;
    private float storedForwardValue = 0f;

    public delegate void ThrusterUpdate(Vector3 value);
    public event ThrusterUpdate MovementUpdate;
    public event ThrusterUpdate RotationUpdate;

    public delegate void BoostUpdate(bool value);
    public event BoostUpdate OnBoost;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.AddRelativeForce(movementValues, ForceMode.Acceleration);
        rb.AddRelativeTorque(rotationValues, ForceMode.Acceleration);

        if (!boosting && rb.velocity.sqrMagnitude > maxVelocity * maxVelocity)
        {
            rb.AddForce(-rb.velocity.normalized * (rb.velocity.magnitude - maxVelocity), ForceMode.Acceleration);
        }
    }

    public void SetForward(float value)
    {
        if (boosting)
        {
            storedForwardValue = value;
            return;
        }

        movementValues.z = value;
        movementValues.z *= (value > 0 ? forwardThrust * boostMod : reverseThrust);

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

        if (boosting)
        {
            storedForwardValue = movementValues.z;
            boostMod = boostMultiplier;
            steerPenalty = boostPenalty;
            movementValues.z = forwardThrust * boostMod;
        }
        else
        {
            boostMod = 1;
            steerPenalty = 1;
            movementValues.z = storedForwardValue;
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
    }
}
