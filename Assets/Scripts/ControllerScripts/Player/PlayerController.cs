using SpaceGame.Utils;
using UnityEngine;
using Weapons;

[RequireComponent(typeof(Destructible))]
public class PlayerController : MovementController
{
    private InputReader _input;

    private Destructible _ship;

    [SerializeField]
    private WeaponSystem _primaryWeapons, _secondaryWeaapons;

    public float idleForwardThrust = 0.2f;  // TODO make a player data object to store this information

    private void Awake()
    {
        _ship = GetComponent<Destructible>();
        gameObject.SetActive(false);
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

        //_input.AimEvent += HandleAimEvent;
        _input.PitchEvent += SetPitch;
        _input.YawEvent += SetYaw;
        _input.RollEvent += SetRoll;

        _input.ThrustEvent += HandleThrustEvent;
        _input.BoostEvent += SetBoost;
        _input.BrakeEvent += SetBrake;

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

        //_input.AimEvent -= HandleAimEvent;
        _input.PitchEvent -= SetPitch;
        _input.YawEvent -= SetYaw;
        _input.RollEvent -= SetRoll;

        _input.ThrustEvent -= HandleThrustEvent;
        _input.BoostEvent -= SetBoost;
        _input.BrakeEvent -= SetBrake;

        _input.PrimaryTriggeredEvent -= _primaryWeapons.OnFiringButtonPressed;
        _input.PrimaryReleasedEvent -= _primaryWeapons.OnFiringButtonReleased;

        _input.SecondaryTriggeredEvent -= _secondaryWeaapons.OnFiringButtonPressed;
        _input.SecondaryTriggeredEvent -= _secondaryWeaapons.OnFiringButtonReleased;
    }

    private void HandleThrustEvent(bool thrusting)
    {
        SetForward(thrusting ? 1f : idleForwardThrust);
    }

    private void HandleAimEvent(Vector2 direction)
    {
        SetPitch(-direction.y);
        SetYaw(direction.x);
    }

    private void OnDeath()
    {
        SetLock(true);
    }
}
