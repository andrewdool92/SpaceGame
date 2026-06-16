using UnityEngine;
using SpaceGame.Utils;
using System.Threading.Tasks;

namespace SpaceGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

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

    }
}