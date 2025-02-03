using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Destructible))]
public class PlayerInputReader : MonoBehaviour, GameInput.IShipControlsActions
{
    private GameInput input;
    private Destructible ship;
    public WeaponSystem mainWeapons, secondaryWeapons;
    public ReticuleController reticuleController;

    public Material aimCircleMaterial;
    private int aimMaterialMask;

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

        aimMaterialMask = Shader.PropertyToID("_CursorPosition");
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
        //reticuleController.onAimAssist += mainWeapons.OnTargetLocked;
        //mainWeapons.SetAimTransform(reticuleController.aimTransform);

        reticuleController.AssignWeapons(mainWeapons, secondaryWeapons);
    }

    private void OnDisable()
    {
        input.ShipControls.RemoveCallbacks(this);
        ship.onDestruction -= OnDeath;
        //reticuleController.onAimAssist -= mainWeapons.OnTargetLocked;
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        float thrust = Mathf.Clamp(context.ReadValue<float>(), idleForwardThrust, 1f);
        movement.SetForward(thrust);
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movement.SetBoost(true);
        }
        else if (context.canceled)
        {
            movement.SetBoost(false);
        }
        //movement.SetBoost(context.performed);
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        Vector2 mousePos = context.ReadValue<Vector2>();
        Vector2 aimValue = mousePos - screenCentre;
        float sqrMagnitude = aimValue.sqrMagnitude;

        aimCircleMaterial.SetVector(aimMaterialMask, aimValue);
        //Debug.Log($"{mousePos} : {aimCircleMaterial.GetVector(aimMaterialMask)}");

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

    public void OnLeftStick(InputAction.CallbackContext context)
    {

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
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed) mainWeapons.OnFiringButtonPressed();
        else if (context.canceled) mainWeapons.OnFiringButtonReleased();
    }
    public void OnBrake(InputAction.CallbackContext context)
    {
        movement.SetBrake(context.performed);
    }

    public void OnDeath()
    {
        input.Disable();
        movement.SetLock(true);
    }

    public void OnSecondary(InputAction.CallbackContext context)
    {
        if (context.performed) secondaryWeapons.OnFiringButtonPressed();
        else if (context.canceled) secondaryWeapons.OnFiringButtonReleased();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
