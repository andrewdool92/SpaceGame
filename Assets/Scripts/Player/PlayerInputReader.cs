using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Destructible))]
public class PlayerInputReader : MonoBehaviour, GameInput.IShipControlsActions
{
    private GameInput input;
    private Destructible ship;

    public int deadZone = 50;
    public int fineTuneRange = 200;
    private bool aiming = false;
    private float minThreshold, maxThreshold;

    public float idleForwardThrust = 0.2f;

    private MovementController movement;

    private Vector2 mousePosition;
    private Vector2 screenCentre;
    private Vector2 aimDirection = Vector2.zero;

    private void Awake()
    {
        input = new GameInput();
        input.Enable();

        movement = GetComponent<MovementController>();
        ship = GetComponent<Destructible>();
    }

    // Start is called before the first frame update
    void Start()
    {
        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        minThreshold = deadZone * deadZone;
        maxThreshold = fineTuneRange * fineTuneRange;

        movement.SetForward(idleForwardThrust);
    }

    private void OnEnable()
    {
        input.ShipControls.AddCallbacks(this);
        ship.onDestruction += OnDeath;
    }

    private void OnDisable()
    {
        input.ShipControls.RemoveCallbacks(this);
        ship.onDestruction -= OnDeath;
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        float thrust = Mathf.Clamp(context.ReadValue<float>(), idleForwardThrust, 1f);
        movement.SetForward(thrust);
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        movement.SetBoost(context.performed);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 aimValue = context.ReadValue<Vector2>() - screenCentre;
        float sqrMagnitude = aimValue.sqrMagnitude;

        if (sqrMagnitude < minThreshold)
        {
            if (aiming)
            {
                aiming = false;
                movement.SetPitch(0);
                movement.SetYaw(0);
            }
            return;
        }

        aimValue = aimValue.normalized;
        if (sqrMagnitude - minThreshold < maxThreshold)
        {
            aimValue *= (sqrMagnitude - minThreshold) / maxThreshold;
        }

        movement.SetPitch(-aimValue.y);
        movement.SetYaw(aimValue.x);
        aiming = true;
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        movement.SetRoll(context.ReadValue<float>());
    }

    public void OnStrafe(InputAction.CallbackContext context) { }
    public void OnUp(InputAction.CallbackContext context)
    {
        movement.SetVertical(context.ReadValue<float>());
    }
    public void OnReverse(InputAction.CallbackContext context) { }
    public void OnShoot(InputAction.CallbackContext context) { }
    public void OnBrake(InputAction.CallbackContext context)
    {
        movement.SetBrake(context.performed);
    }

    public void OnDeath()
    {
        input.Disable();
        movement.SetLock(true);
    }
}
