using System;
using UnityEngine;

namespace SpaceGame.Utils
{
    public class CameraUtils : MonoBehaviour
    {
        private static CameraUtils instance;

        private Vector2 _screenResolution;
        public static Vector2 ScreenResolution
        {
            get
            {
                return instance._screenResolution;
            }
        }

        public static event Action<Vector2> ScreenSizeChangedEvent;


        [SerializeField]
        private Camera _UICamera;
        public static Camera UICamera
        {
            get
            {
                return instance._UICamera;
            }
        }

        private void Awake()
        {
            instance = this;

            _screenResolution = new Vector3(Screen.width, Screen.height);
        }

        private void Update()
        {
            if (Screen.width != _screenResolution.x || Screen.height != _screenResolution.y)
            {
                _screenResolution.x = Screen.width;
                _screenResolution.y = Screen.height;

                ScreenSizeChangedEvent?.Invoke(_screenResolution);
            }
        }
    }
}
