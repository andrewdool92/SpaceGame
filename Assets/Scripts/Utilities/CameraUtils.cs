using UnityEngine;

namespace SpaceGame.Utils
{
    public class CameraUtils : MonoBehaviour
    {
        private static CameraUtils instance;

        private void Awake()
        {
            instance = this;
        }

        [SerializeField]
        private Camera _UICamera;

        public static Camera UICamera
        {
            get
            {
                return instance._UICamera;
            }
        }
    }
}
