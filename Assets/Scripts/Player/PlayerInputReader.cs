using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementController))]
public class PlayerInputReader : MonoBehaviour
{
    public InputActionAsset inputActions;

    public int deadZone = 50;
    public int fineTuneRange = 100;
    private bool aiming = false;
    private float minThreshold, maxThreshold;

    private MovementController movement;

    private Vector2 mousePosition;
    private Vector2 screenCentre;
    private Vector2 aimDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();

        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        minThreshold = deadZone * deadZone;
        maxThreshold = fineTuneRange * fineTuneRange;
    }

    private void OnForward(InputAction.CallbackContext context)
    {
        movement.SetForward(context.ReadValue<float>());
    }

    private void OnBoost(InputAction.CallbackContext context)
    {
        movement.SetBoost(context.ReadValue<bool>());
    }

    private void HandleAim(InputAction.CallbackContext context)
    {
        Vector2 aimValue = context.ReadValue<Vector2>() - screenCentre;
        float sqrMagnitude = aimValue.sqrMagnitude;

        if (aiming && sqrMagnitude < minThreshold)
        {
            aiming = false;
            movement.SetPitch(0);
            movement.SetYaw(0);
            return;
        }

        aimValue = aimValue.normalized;
        if (sqrMagnitude - minThreshold < maxThreshold)
        {
            aimValue *= (sqrMagnitude - minThreshold) / maxThreshold;
        }

        movement.SetPitch(aimValue.y);
        movement.SetYaw(aimValue.x);
        aiming = true;
    }

    private void HandleRoll(InputAction.CallbackContext context)
    {
        movement.SetRoll(context.ReadValue<float>());
    }
}
