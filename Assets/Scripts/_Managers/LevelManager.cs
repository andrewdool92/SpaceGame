using NUnit.Framework;
using UnityEngine;

namespace SpaceGame.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _playerStartPoint;

        private PlayerController player;

        public async Awaitable Setup()
        {
            // load player
            player = await SceneManagerUtils.LoadPlayer();
            player.transform.SetPositionAndRotation(_playerStartPoint.position, _playerStartPoint.rotation);


        }

        public async Awaitable Run()
        {

        }

        public async Awaitable Teardown()
        {
            // unload sub scenes (tunnels, etc.)
            // unload player scene
        }
    }
}
