using UnityEngine;

namespace SpaceGame.Utils
{
    public class InputSettings
    {
        private string _mouseDeadZoneKey = "mouseDeadZone";
        private float _mouseDeadZone = 75;
        public float SqrMouseDeadZone { get; private set; }
        public float MouseDeadZone {
            get => _mouseDeadZone;
            set
            {
                if (_mouseDeadZone != value)
                {
                    _mouseDeadZone = value;
                    SqrMouseDeadZone = value * value;
                    PlayerPrefs.SetFloat(_mouseDeadZoneKey, value);
                }
            }
        }

        private string _mouseTuneKey = "mouseTuneRange";
        private float _mouseTuneRange = 200;
        public float SqrMouseTuneRange { get; private set; }
        public float MouseTuneRange
        {
            get => _mouseTuneRange;
            set
            {
                if ( _mouseTuneRange != value)
                {
                    _mouseTuneRange = value;
                    SqrMouseTuneRange = value * value;
                    PlayerPrefs.SetFloat(_mouseTuneKey, value);
                }
            }
        }

        private string _stickDeadZoneKey = "stickDeadZone";
        private float _stickDeadZone = 0.125f;
        public float StickDeadZone { get => _stickDeadZone; }

        public InputSettings()
        {
            if (PlayerPrefs.HasKey(_mouseDeadZoneKey))
            {
                _mouseDeadZone = PlayerPrefs.GetFloat(_mouseDeadZoneKey);
            }

            if (PlayerPrefs.HasKey(_mouseTuneKey))
            {
                _mouseTuneRange = PlayerPrefs.GetFloat(_mouseTuneKey);
            }

            if (PlayerPrefs.HasKey(_stickDeadZoneKey))
            {
                _stickDeadZone = PlayerPrefs.GetFloat(_stickDeadZoneKey);
            }
        }
    }
}
