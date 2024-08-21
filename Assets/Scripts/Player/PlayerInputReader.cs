using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementController))]
public class PlayerInputReader : MonoBehaviour, GameInput.IShipControlsActions
{
    private GameInput input;

    public int deadZone = 50;
    public int fineTuneRange = 100;
    private bool aiming = false;
    private float minThreshold, maxThreshold;

    private MovementController movement;

    private Vector2 mousePosition;
    private Vector2 screenCentre;
    private Vector2 aimDirection = Vector2.zero;

    private void Awake()
    {
        input = new GameInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();

        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        minThreshold = deadZone * deadZone;
        maxThreshold = fineTuneRange * fineTuneRange;
    }

    private void OnEnable()
    {
        input.ShipControls.AddCallbacks(this);
    }

    private void OnDisable()
    {
        input.ShipControls.RemoveCallbacks(this);
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        movement.SetForward(context.ReadValue<float>());
        Debug.Log(context.phase);
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        movement.SetBoost(context.ReadValue<bool>());
    }

    public void OnAim(InputAction.CallbackContext context)
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

    public void OnRoll(InputAction.CallbackContext context)
    {
        movement.SetRoll(context.ReadValue<float>());
    }

    public void OnStrafe(InputAction.CallbackContext context) { }
    public void OnUp(InputAction.CallbackContext context) { }
    public void OnReverse(InputAction.CallbackContext context) { }
    public void OnShoot(InputAction.CallbackContext context) { }
    public void OnBrake(InputAction.CallbackContext context) { }
}
