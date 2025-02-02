using UnityEngine;

namespace SpaceGame.Managers
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;

        private void Awake()
        {
            instance = this;
        }

        [SerializeField]
        private Camera _UICamera;

        public static Camera UICamera { get
            {
                return instance._UICamera;
            } }
    }
}
