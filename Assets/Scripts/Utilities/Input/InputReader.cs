using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceGame.Utils
{
    public class InputReader : GameInput.IShipControlsActions, GameInput.IUIActions
    {
        private readonly GameInput _gameInput;
        private readonly InputSettings _settings;

        public event Action<Vector2> MouseMoveEvent;
        public event Action<Vector2> AimEvent;
        public event Action<float> PitchEvent;
        public event Action<float> RollEvent;
        public event Action<float> YawEvent;
        private Vector2 _screenCentre;
        private Vector2 _mousePos;

        public event Action<bool> BrakeEvent;
        public event Action<bool> ThrustEvent;
        public event Action<bool> BoostEvent;

        public event Action PrimaryTriggeredEvent;
        public event Action PrimaryReleasedEvent;

        public event Action SecondaryTriggeredEvent;
        public event Action SecondaryReleasedEvent;

        public event Action PauseEvent;
        public event Action ResumeEvent;

        public InputReader(InputSettings settings)
        {
            _gameInput = new GameInput();
            _settings = settings;

            _gameInput.ShipControls.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);

            UpdateScreenCentre(CameraUtils.ScreenResolution);
            CameraUtils.ScreenSizeChangedEvent += UpdateScreenCentre;

            SetGameplay();
        }

        public void SetGameplay()
        {
            _gameInput.ShipControls.Enable();
            _gameInput.UI.Disable();
        }

        public void SetUI()
        {
            _gameInput.ShipControls.Disable();
            _gameInput.UI.Enable();
        }

        public void UpdateScreenCentre(Vector2 resolution)
        {
            _screenCentre = resolution / 2;
        }

        // Ship Control events
        public void OnMouse(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input == _screenCentre)
            {
                return;
            }

            Cursor.visible = true;
            _mousePos = input;
            MouseMoveEvent?.Invoke(input);

            Vector2 direction = input - _screenCentre;
            float magnitude = Mathf.Clamp(direction.magnitude - _settings.MouseDeadZone, 0, _settings.MouseTuneRange) / _settings.MouseTuneRange;
            input = direction.normalized * magnitude;

            //AimEvent?.Invoke(input);
            PitchEvent?.Invoke(-input.y);
            YawEvent?.Invoke(input.x);
        }

        public void OnLeftStick(InputAction.CallbackContext context)
        {
            if (_mousePos != _screenCentre)
            {
                // this may be better left to a setting to be intentionally toggled
                // rather than automatically switching between kbm and controller when inputs are detected?
                Mouse.current.WarpCursorPosition(_screenCentre);
                Cursor.visible = false;
            }

            Vector2 input = context.ReadValue<Vector2>();
            if (input.magnitude < _settings.StickDeadZone)
            {
                input = Vector2.zero;
            }
            else if (Mathf.Abs(input.x) < 0.3)
            {
                input.x = 0;
            }

            //PitchEvent?.Invoke(-input.x);
            //RollEvent?.Invoke(-input.y);
            ThrustEvent?.Invoke(input.y > 0);
            BrakeEvent?.Invoke(input.y < 0);
            YawEvent?.Invoke(input.x);
        }

        public void OnRightStick(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input.magnitude < _settings.StickDeadZone)
            {
                input = Vector2.zero;
            }
            PitchEvent?.Invoke(input.y);
            RollEvent?.Invoke(-input.x * Mathf.Abs(input.x));
        }

        public void OnBoost(InputAction.CallbackContext context)
        {
            BoostEvent?.Invoke(context.performed);
        }

        public void OnBrake(InputAction.CallbackContext context)
        {
            BrakeEvent?.Invoke(context.performed);
        }

        public void OnForward(InputAction.CallbackContext context)
        {
            ThrustEvent?.Invoke(context.performed);
        }

        public void OnReverse(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnRoll(InputAction.CallbackContext context)
        {
            RollEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed: PrimaryTriggeredEvent?.Invoke(); break;
                case InputActionPhase.Canceled: PrimaryReleasedEvent?.Invoke(); break;
            }
        }

        public void OnSecondary(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed: SecondaryTriggeredEvent?.Invoke(); break;
                case InputActionPhase.Canceled: SecondaryReleasedEvent?.Invoke(); break;
            }
        }

        public void OnStrafe(InputAction.CallbackContext context)
        {
            
        }

        public void OnUp(InputAction.CallbackContext context)
        {
            
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PauseEvent?.Invoke();
            }
        }

        // UI events
        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ResumeEvent?.Invoke();
            }
        }
    }
}
