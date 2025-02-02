using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceGame.Managers
{
    public static class SceneManagerUtils
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void OnApplicationStart()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "PersistentScene")
            {
                await SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive);
            }
        }

        public static async void LoadAndUnloadScene(string toLoad, string toUnload)
        {
            await SceneManager.LoadSceneAsync(toLoad, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(toLoad));
            await SceneManager.UnloadSceneAsync(toUnload);
        }
    }
}
