using SpaceGame.Utils;
using UnityEngine;
using Weapons;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Destructible))]
public class PlayerController : MonoBehaviour
{
    private InputReader _input;

    private MovementController _movementController;
    private Destructible _ship;

    [SerializeField]
    private WeaponSystem _primaryWeapons, _secondaryWeaapons;

    public float idleForwardThrust = 0.2f;  // TODO make a player data object to store this information

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _ship = GetComponent<Destructible>();
    }

    public void Init(InputReader gameInput)
    {
        _input = gameInput;

        _ship.onDestruction += OnDeath;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        RegisterInputEvents();
    }

    private void OnDisable()
    {
        UnregisterInputEvents();
    }

    private void OnDestroy()
    {
        _ship.onDestruction -= OnDeath;
    }

    private void RegisterInputEvents()
    {
        if (ReferenceEquals(_input, null))
        {
            return;
        }

        _input.AimEvent += HandleAimEvent;
        _input.ThrustEvent += HandleThrustEvent;
        _input.BoostEvent += HandleBoostEvent;
        _input.BrakeEvent += HandleBrakeEvent;

        _input.PrimaryTriggeredEvent += _primaryWeapons.OnFiringButtonPressed;
        _input.PrimaryReleasedEvent += _primaryWeapons.OnFiringButtonReleased;

        _input.SecondaryTriggeredEvent += _secondaryWeaapons.OnFiringButtonPressed;
        _input.SecondaryTriggeredEvent += _secondaryWeaapons.OnFiringButtonReleased;
    }

    private void UnregisterInputEvents()
    {
        if (ReferenceEquals(_input, null))
        {
            return;
        }

        _input.AimEvent -= HandleAimEvent;
        _input.ThrustEvent -= HandleThrustEvent;
        _input.BoostEvent -= HandleBoostEvent;
        _input.BrakeEvent -= HandleBrakeEvent;

        _input.PrimaryTriggeredEvent -= _primaryWeapons.OnFiringButtonPressed;
        _input.PrimaryReleasedEvent -= _primaryWeapons.OnFiringButtonReleased;

        _input.SecondaryTriggeredEvent -= _secondaryWeaapons.OnFiringButtonPressed;
        _input.SecondaryTriggeredEvent -= _secondaryWeaapons.OnFiringButtonReleased;
    }

    private void HandleThrustEvent(bool thrusting)
    {
        _movementController.SetForward(thrusting ? 1f : idleForwardThrust);
    }

    private void HandleBrakeEvent(bool braking)
    {
        _movementController.SetBrake(braking);
    }

    private void HandleBoostEvent(bool boosting)
    {
        _movementController.SetBoost(boosting);
    }

    private void HandleAimEvent(Vector2 direction)
    {
        _movementController.SetPitch(-direction.y);
        _movementController.SetYaw(direction.x);
    }

    private void OnDeath()
    {
        _movementController.SetLock(true);
    }
}
