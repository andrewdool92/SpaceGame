using UnityEngine;
using SpaceGame.Utils;

namespace SpaceGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public InputSettings InputSettings;
        public InputReader InputReader;


        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            InputSettings = new();
            InputReader = new(InputSettings);
        }

        public void OnPlayerLoaded()
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
        }
    }
}