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

            LoadPlayer();
        }

        public async void LoadPlayer()
        {
            await SceneManagerUtils.LoadPlayer(Vector3.zero);

            PlayerController player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            player.Init(InputReader);
        }
    }
}