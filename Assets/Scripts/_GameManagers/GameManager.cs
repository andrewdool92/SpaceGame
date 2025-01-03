using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static async void OnApplicationStart()
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "PersistentScene")
        {
            await SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive);
        }
    }
}
