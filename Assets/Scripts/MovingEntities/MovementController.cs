using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public float pitchTorque, yawTorque, rollTorque, verticalThrust, strafeThrust, forwardThrust, reverseThrust, boostMultiplier, brakeDrag;
    public float boostPenalty;

    private Vector3 movementValues = Vector3.zero;
    private Vector3 rotationValues = Vector3.zero;

    private Rigidbody rb;

    private bool boosting = false;
    private float steerPenalty = 1;
    private float boostMod = 1;

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
        rb.AddRelativeForce(movementValues);
        rb.AddRelativeTorque(rotationValues);
    }

    public void SetForward(float value)
    {
        if (boosting && value <= 0) SetBoost(false);

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
            boostMod = boostMultiplier;
            steerPenalty = boostPenalty;
            movementValues.z = forwardThrust * boostMod;
        }
        else
        {
            boostMod = 1;
            steerPenalty = 1;
        }

        OnBoost?.Invoke(value);
    }

    public void SetBrake(bool braking)
    {
        rb.drag = braking ? brakeDrag : 0;
    }
}
